using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDefaultNullSelectedMarkTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_DisciplinesTypes_DisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes");

            migrationBuilder.DropIndex(
                name: "IX_SelectedMarkTypes_DisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropColumn(
                name: "DisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.AlterColumn<int>(
                name: "DisciplineId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisciplinesTypesDisciplineTypeId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes",
                columns: new[] { "LessonTypeId", "MarkTypeId", "DisciplineId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                table: "SelectedMarkTypes",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "DisciplineId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_DisciplinesTypes_DisciplinesTypesDiscipli~",
                table: "SelectedMarkTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes");

            migrationBuilder.DropIndex(
                name: "IX_SelectedMarkTypes_DisciplinesTypesDisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropColumn(
                name: "DisciplinesTypesDisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.AlterColumn<int>(
                name: "DisciplineId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "DisciplineTypeId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes",
                columns: new[] { "LessonTypeId", "MarkTypeId", "DisciplineTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedMarkTypes_DisciplineTypeId",
                table: "SelectedMarkTypes",
                column: "DisciplineTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedMarkTypes_DisciplinesTypes_DisciplineTypeId",
                table: "SelectedMarkTypes",
                column: "DisciplineTypeId",
                principalTable: "DisciplinesTypes",
                principalColumn: "DisciplineTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                table: "SelectedMarkTypes",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "DisciplineId");
        }
    }
}
