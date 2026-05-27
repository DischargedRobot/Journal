using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class DefaultZeroLessonMarkTypeSelectedMarkTypesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_DisciplinesTypes_DisciplinesTypesDiscipli~",
                table: "SelectedMarkTypes");

            migrationBuilder.DropIndex(
                name: "IX_SelectedMarkTypes_DisciplinesTypesDisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropColumn(
                name: "DisciplinesTypesDisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.AlterColumn<int>(
                name: "MarkTypeId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "LessonTypeId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MarkTypeId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "LessonTypeId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisciplinesTypesDisciplineTypeId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelectedMarkTypes_DisciplinesTypesDisciplineTypeId",
                table: "SelectedMarkTypes",
                column: "DisciplinesTypesDisciplineTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedMarkTypes_DisciplinesTypes_DisciplinesTypesDiscipli~",
                table: "SelectedMarkTypes",
                column: "DisciplinesTypesDisciplineTypeId",
                principalTable: "DisciplinesTypes",
                principalColumn: "DisciplineTypeId");
        }
    }
}
