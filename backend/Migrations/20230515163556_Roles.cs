using Microsoft.EntityFrameworkCore.Migrations;

namespace backend.Migrations
{
    public partial class Roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "d44e53bf-3c7e-41d0-99bb-190da08028d2", "811011d8-7931-421c-bb59-9c97d65571f1", "User", "USER" },
                    { "0feec70b-f079-4d9b-93ea-1adf4f3c6984", "3f1c9305-6f5c-4978-a30c-5c387fbbd0e2", "Administrator", "ADMINISTRATOR" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0feec70b-f079-4d9b-93ea-1adf4f3c6984");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d44e53bf-3c7e-41d0-99bb-190da08028d2");
        }
    }
}
