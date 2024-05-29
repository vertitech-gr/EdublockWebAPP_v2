using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class includesUniversityDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UniversityDetails_UniversityId",
                table: "UniversityDetails",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityDetails_Universities_UniversityId",
                table: "UniversityDetails",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniversityDetails_Universities_UniversityId",
                table: "UniversityDetails");

            migrationBuilder.DropIndex(
                name: "IX_UniversityDetails_UniversityId",
                table: "UniversityDetails");
        }
    }
}
