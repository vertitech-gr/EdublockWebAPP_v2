using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class AdminOTPAndUniversityUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Universities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Otp",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Email",
                table: "Universities",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Id",
                table: "Universities",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Otp_AdminId",
                table: "Otp",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Otp_Admin_AdminId",
                table: "Otp",
                column: "AdminId",
                principalTable: "Admin",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otp_Admin_AdminId",
                table: "Otp");

            migrationBuilder.DropIndex(
                name: "IX_Universities_Email",
                table: "Universities");

            migrationBuilder.DropIndex(
                name: "IX_Universities_Id",
                table: "Universities");

            migrationBuilder.DropIndex(
                name: "IX_Otp_AdminId",
                table: "Otp");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Otp");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Universities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
