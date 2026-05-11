using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdDisiplinesRegistr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisciplineId",
                table: "DisciplinesRegisters",
                newName: "DisciplineRegisterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisciplineRegisterId",
                table: "DisciplinesRegisters",
                newName: "DisciplineId");
        }
    }
}
