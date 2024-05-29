using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class AddedLoginStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "loginStatus",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "loginStatus",
                table: "Universities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "loginStatus",
                table: "Employers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "loginStatus",
                table: "Departments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "loginStatus",
                table: "Admin",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loginStatus",
                table: "User");

            migrationBuilder.DropColumn(
                name: "loginStatus",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "loginStatus",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "loginStatus",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "loginStatus",
                table: "Admin");
        }
    }
}
