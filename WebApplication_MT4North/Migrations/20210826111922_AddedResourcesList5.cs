using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication_MT4North.Migrations
{
    public partial class AddedResourcesList5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Resources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Resources");
        }
    }
}
