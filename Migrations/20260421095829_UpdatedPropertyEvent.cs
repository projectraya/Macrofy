using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Macrofy.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPropertyEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfirmedSpots",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedSpots",
                table: "Events");
        }
    }
}
