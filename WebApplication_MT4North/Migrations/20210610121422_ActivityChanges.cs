using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication_MT4North.Migrations
{
    public partial class ActivityChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomActivityInfos_Activities_ActivityId",
                table: "CustomActivityInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_UserId1",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId1",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_CustomActivityInfos_ActivityId",
                table: "CustomActivityInfos");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Notes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Notes",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomActivityInfoId",
                table: "Activities",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomActivityInfoId1",
                table: "Activities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_UserId",
                table: "Notes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_CustomActivityInfos_CustomActivityInfoId1",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_UserId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Activities_CustomActivityInfoId1",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CustomActivityInfoId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CustomActivityInfoId1",
                table: "Activities");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Notes",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Notes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId1",
                table: "Notes",
                column: "UserId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_UserId1",
                table: "Notes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
