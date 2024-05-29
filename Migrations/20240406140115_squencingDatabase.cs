using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class squencingDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_Permissions_PermissionId",
                table: "RolePermissionMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_PermissionDetails_PermissionId",
                table: "RolePermissionMappings",
                column: "PermissionId",
                principalTable: "PermissionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionMappings_PermissionDetails_PermissionId",
                table: "RolePermissionMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionMappings_Permissions_PermissionId",
                table: "RolePermissionMappings",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
