using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class updateROlemapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Roles_RoleId",
                table: "RolePermissionMappings");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissionMappings_RoleId",
                table: "RolePermissionMappings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionMappings_RoleId",
                table: "RolePermissionMappings",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Roles_RoleId",
                table: "RolePermissionMappings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
