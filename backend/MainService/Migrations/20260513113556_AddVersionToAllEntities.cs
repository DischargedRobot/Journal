using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_DisciplinesRegisters_DisciplineRegisterId",
                table: "Disciplines");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "UniversityEmployers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TrainingDirections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "StudentPersons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "StudentNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Semesters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Professors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "PresenceStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "MarkTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Marks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "LessonTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "LessonPresences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "LessonMarks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Faculties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "EmployeePosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "DisciplinesRegisters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "DisciplineRegisterId",
                table: "Disciplines",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Disciplines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Departments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Curators",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Brigades",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AttestationTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Attestations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AttestationMarks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AcademicYears",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_DisciplinesRegisters_DisciplineRegisterId",
                table: "Disciplines",
                column: "DisciplineRegisterId",
                principalTable: "DisciplinesRegisters",
                principalColumn: "DisciplineRegisterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_DisciplinesRegisters_DisciplineRegisterId",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "UniversityEmployers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "TrainingDirections");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "StudentPersons");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "StudentNotes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Semesters");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SelectedMarkTypes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PresenceStatuses");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "MarkTypes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "LessonTypes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "LessonPresences");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "LessonMarks");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "EmployeePosts");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "DisciplinesRegisters");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Curators");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Brigades");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AttestationTypes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Attestations");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AttestationMarks");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AcademicYears");

            migrationBuilder.AlterColumn<int>(
                name: "DisciplineRegisterId",
                table: "Disciplines",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_DisciplinesRegisters_DisciplineRegisterId",
                table: "Disciplines",
                column: "DisciplineRegisterId",
                principalTable: "DisciplinesRegisters",
                principalColumn: "DisciplineRegisterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
