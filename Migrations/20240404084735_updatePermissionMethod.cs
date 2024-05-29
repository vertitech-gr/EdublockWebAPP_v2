using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edu_Block_dev.Migrations
{
    /// <inheritdoc />
    public partial class updatePermissionMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ControllerModule",
                table: "Permissions",
                newName: "Module");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Module",
                table: "Permissions",
                newName: "ControllerModule");
        }
    }
}
