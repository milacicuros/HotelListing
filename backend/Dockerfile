# Build Stage

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /backend
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Serve Stage

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /backend
COPY --from=build-env /backend/out .

EXPOSE 5001

ENTRYPOINT ["dotnet", "backend.dll"]
