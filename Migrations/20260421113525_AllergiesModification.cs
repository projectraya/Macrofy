using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Macrofy.Migrations
{
    /// <inheritdoc />
    public partial class AllergiesModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAllergies",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

			migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers""
                SET ""HasAllergies"" = NOT ""NoAllergies""
                ");
		    }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAllergies",
                table: "AspNetUsers");
        }
    }
}
