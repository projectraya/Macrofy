using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Macrofy.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedChefProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventBadgeLabel",
                table: "ChefProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasEventBadge",
                table: "ChefProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventBadgeLabel",
                table: "ChefProfiles");

            migrationBuilder.DropColumn(
                name: "HasEventBadge",
                table: "ChefProfiles");
        }
    }
}
