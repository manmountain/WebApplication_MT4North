using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication_MT4North.Migrations
{
    public partial class ActivityChangesV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_CustomActivityInfos_CustomActivityInfoId1",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_CustomActivityInfoId1",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CustomActivityInfoId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CustomActivityInfoId1",
                table: "Activities");

            migrationBuilder.CreateIndex(
                name: "IX_CustomActivityInfos_ActivityId",
                table: "CustomActivityInfos",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomActivityInfos_Activities_ActivityId",
                table: "CustomActivityInfos",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomActivityInfos_Activities_ActivityId",
                table: "CustomActivityInfos");

            migrationBuilder.DropIndex(
                name: "IX_CustomActivityInfos_ActivityId",
                table: "CustomActivityInfos");

            migrationBuilder.AddColumn<int>(
                name: "CustomActivityInfoId",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomActivityInfoId1",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CustomActivityInfoId1",
                table: "Activities",
                column: "CustomActivityInfoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_CustomActivityInfos_CustomActivityInfoId1",
                table: "Activities",
                column: "CustomActivityInfoId1",
                principalTable: "CustomActivityInfos",
                principalColumn: "CustomActivityInfoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
