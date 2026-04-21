using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Macrofy.Migrations
{
    /// <inheritdoc />
    public partial class AddDietaryNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DietaryNotes",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietaryNotes",
                table: "AspNetUsers");
        }
    }
}
