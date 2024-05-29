using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class AllOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otp_Admin_AdminId",
                table: "Otp");

            migrationBuilder.DropIndex(
                name: "IX_Otp_AdminId",
                table: "Otp");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Otp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Otp",
                type: "uniqueidentifier",
                nullable: true);

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
    }
}
