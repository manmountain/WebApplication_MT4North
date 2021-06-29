using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication_MT4North.Migrations
{
    public partial class ActivityChangesV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_BaseActivityInfos_BaseInfoId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomActivityInfos_Activities_ActivityId",
                table: "CustomActivityInfos");

            migrationBuilder.DropIndex(
                name: "IX_CustomActivityInfos_ActivityId",
                table: "CustomActivityInfos");

            migrationBuilder.DropIndex(
                name: "IX_Activities_BaseInfoId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "CustomActivityInfos");

            migrationBuilder.DropColumn(
                name: "BaseInfoId",
                table: "Activities");

            migrationBuilder.AddColumn<int>(
                name: "BaseActivityInfoId",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomActivityInfoId",
                table: "Activities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_BaseActivityInfoId",
                table: "Activities",
                column: "BaseActivityInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CustomActivityInfoId",
                table: "Activities",
                column: "CustomActivityInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_BaseActivityInfos_BaseActivityInfoId",
                table: "Activities",
                column: "BaseActivityInfoId",
                principalTable: "BaseActivityInfos",
                principalColumn: "BaseActivityInfoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_CustomActivityInfos_CustomActivityInfoId",
                table: "Activities",
                column: "CustomActivityInfoId",
                principalTable: "CustomActivityInfos",
                principalColumn: "CustomActivityInfoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_BaseActivityInfos_BaseActivityInfoId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_CustomActivityInfos_CustomActivityInfoId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_BaseActivityInfoId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_CustomActivityInfoId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "BaseActivityInfoId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CustomActivityInfoId",
                table: "Activities");

            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "CustomActivityInfos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseInfoId",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomActivityInfos_ActivityId",
                table: "CustomActivityInfos",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_BaseInfoId",
                table: "Activities",
                column: "BaseInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_BaseActivityInfos_BaseInfoId",
                table: "Activities",
                column: "BaseInfoId",
                principalTable: "BaseActivityInfos",
                principalColumn: "BaseActivityInfoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomActivityInfos_Activities_ActivityId",
                table: "CustomActivityInfos",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
