using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class AddedIssuerInCertifcate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Issuer",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Issuer",
                table: "Certificates");
        }
    }
}
