using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Server.Migrations
{
    /// <inheritdoc />
    public partial class ForceUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempColumn",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempColumn",
                table: "Users");
        }
    }
}
