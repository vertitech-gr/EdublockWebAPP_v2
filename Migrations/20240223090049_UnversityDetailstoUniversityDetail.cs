using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class UnversityDetailstoUniversityDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UniversityDetails_UniversityId",
                table: "UniversityDetails");

            migrationBuilder.CreateIndex(
                name: "IX_UniversityDetails_UniversityId",
                table: "UniversityDetails",
                column: "UniversityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UniversityDetails_UniversityId",
                table: "UniversityDetails");

            migrationBuilder.CreateIndex(
                name: "IX_UniversityDetails_UniversityId",
                table: "UniversityDetails",
                column: "UniversityId");
        }
    }
}
