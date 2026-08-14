using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTitansManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoggedIn",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "Created", "Deleted", "DiscordName", "MemberSince", "PlayerId" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Admin", new DateOnly(2026, 8, 14), null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "Deleted", "IsActive", "MemberId", "PasswordHash", "Role", "UserName" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("00000000-0000-0000-0000-000000000001"), "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", 0, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Members",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.AddColumn<bool>(
                name: "LoggedIn",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
