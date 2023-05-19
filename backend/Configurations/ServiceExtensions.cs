using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using backend.Data;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace backend.Configurations
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services) {
            var builder = services.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true);
            
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration) {
            var jwtSettings = Configuration.GetSection("Jwt");
            // KEY is variable for jwt key defined in cmd
            //var key = Environment.GetEnvironmentVariable("KEY");

            //adding authentication with jwt as default scheme
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("KEY").Value))
                };
            });
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app) {
            app.UseExceptionHandler(error => {
                error.Run(async context => {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null) {
                        Log.Error($"Something went wrong in the {contextFeature.Error}");
                        await context.Response.WriteAsync(new ErrorHandler {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal server error. Please try again later."
                        }.ToString());
                    }
                });
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services) {
            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) {
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(
                (expirationOpt) => {
                    expirationOpt.MaxAge = 120;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validationOpt) => {
                    validationOpt.MustRevalidate = true;
                }
            );
        }
    
        public static void ConfigureRateLimiting(this IServiceCollection services) {
            var rateLimitRules = new List<RateLimitRule> {
                new RateLimitRule {
                    Endpoint = "*",
                    Limit = 1,
                    Period = "5s"
                }
            };
            services.Configure<IpRateLimitOptions>(options => {
                options.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}