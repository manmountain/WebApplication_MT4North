using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication_MT4North.Migrations
{
    public partial class ThemeChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Themes_ThemeId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_ThemeId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ThemeId",
                table: "Activities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThemeId",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ThemeId",
                table: "Activities",
                column: "ThemeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Themes_ThemeId",
                table: "Activities",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "ThemeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
