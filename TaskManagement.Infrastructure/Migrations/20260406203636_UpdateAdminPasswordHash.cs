using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b8e7d5a1-c2b3-4e5f-a6b7-c8d9e0f1a2b3"),
                column: "PasswordHash",
                value: "$2a$11$8vS5yE8.O8j7V1Z/K/R2.ue1V9yS9k9yS9k9yS9k9yS9k9yS9k9yS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b8e7d5a1-c2b3-4e5f-a6b7-c8d9e0f1a2b3"),
                column: "PasswordHash",
                value: "$2a$11$evS.J.S6.oX.l.X.l.X.l.O8j7V1Z/K/R2.y.y.y.y.y.y.y.y");
        }
    }
}
