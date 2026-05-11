using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdNameDisciplinesRegisters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_TrainingDirections_TrainingDirectionsTrainingDirec~",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TrainingDirectionsTrainingDirectionId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TrainingDirectionsTrainingDirectionId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "DisciplineName",
                table: "DisciplinesRegisters",
                newName: "ShortName");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "LessonTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LessonId",
                table: "LessonMarks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DisciplinesRegisters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "LessonTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DisciplinesRegisters");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "DisciplinesRegisters",
                newName: "DisciplineName");

            migrationBuilder.AddColumn<int>(
                name: "TrainingDirectionsTrainingDirectionId",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LessonId",
                table: "LessonMarks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Students_TrainingDirectionsTrainingDirectionId",
                table: "Students",
                column: "TrainingDirectionsTrainingDirectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_TrainingDirections_TrainingDirectionsTrainingDirec~",
                table: "Students",
                column: "TrainingDirectionsTrainingDirectionId",
                principalTable: "TrainingDirections",
                principalColumn: "TrainingDirectionId");
        }
    }
}
