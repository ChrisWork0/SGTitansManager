using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTitansManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddValueComparerInChampionTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "Champions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "Tags",
                table: "Champions",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
