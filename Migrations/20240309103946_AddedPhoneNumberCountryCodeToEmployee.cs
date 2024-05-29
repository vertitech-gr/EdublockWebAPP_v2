using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhoneNumberCountryCodeToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Employers");
        }
    }
}
