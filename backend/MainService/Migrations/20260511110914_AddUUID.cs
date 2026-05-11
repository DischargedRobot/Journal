using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class AddUUID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes");

            migrationBuilder.DropIndex(
                name: "IX_SelectedMarkTypes_LessonTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks");

            migrationBuilder.RenameColumn(
                name: "StudentNoteId",
                table: "StudentNotes",
                newName: "NotesAboutStudentId");

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "UniversityEmployers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "TrainingDirections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Students",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "StudentPersons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "StudentNotes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Semesters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Professors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "PresenceStatuses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "MarkTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Marks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "LessonTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Lessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "LessonPresences",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "LessonId",
                table: "LessonMarks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "LessonMarkId",
                table: "LessonMarks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "LessonMarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Faculties",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "EmployeePosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "DisciplinesRegisters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Disciplines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Departments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Brigades",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "AttestationTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Attestations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "AttestationMarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "AcademicYears",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_Uuid",
                table: "Users",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UniversityEmployers_Uuid",
                table: "UniversityEmployers",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TrainingDirections_Uuid",
                table: "TrainingDirections",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Students_Uuid",
                table: "Students",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StudentPersons_Uuid",
                table: "StudentPersons",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StudentNotes_Uuid",
                table: "StudentNotes",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Semesters_Uuid",
                table: "Semesters",
                column: "Uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes",
                columns: new[] { "LessonTypeId", "MarkTypeId", "DisciplineId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Professors_Uuid",
                table: "Professors",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_PresenceStatuses_Uuid",
                table: "PresenceStatuses",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MarkTypes_Uuid",
                table: "MarkTypes",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Marks_Uuid",
                table: "Marks",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_LessonTypes_Uuid",
                table: "LessonTypes",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Lessons_Uuid",
                table: "Lessons",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_LessonPresences_Uuid",
                table: "LessonPresences",
                column: "Uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks",
                columns: new[] { "LessonMarkId", "MarkId", "StudentId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Groups_Uuid",
                table: "Groups",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Faculties_Uuid",
                table: "Faculties",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_EmployeePosts_Uuid",
                table: "EmployeePosts",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DisciplinesRegisters_Uuid",
                table: "DisciplinesRegisters",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Disciplines_Uuid",
                table: "Disciplines",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Departments_Uuid",
                table: "Departments",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Brigades_Uuid",
                table: "Brigades",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AttestationTypes_Uuid",
                table: "AttestationTypes",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Attestations_Uuid",
                table: "Attestations",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AttestationMarks_Uuid",
                table: "AttestationMarks",
                column: "Uuid");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AcademicYears_Uuid",
                table: "AcademicYears",
                column: "Uuid");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedMarkTypes_DisciplineId",
                table: "SelectedMarkTypes",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonMarks_LessonId",
                table: "LessonMarks",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_Uuid",
                table: "Users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UniversityEmployers_Uuid",
                table: "UniversityEmployers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TrainingDirections_Uuid",
                table: "TrainingDirections");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Students_Uuid",
                table: "Students");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StudentPersons_Uuid",
                table: "StudentPersons");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StudentNotes_Uuid",
                table: "StudentNotes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Semesters_Uuid",
                table: "Semesters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes");

            migrationBuilder.DropIndex(
                name: "IX_SelectedMarkTypes_DisciplineId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Professors_Uuid",
                table: "Professors");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_PresenceStatuses_Uuid",
                table: "PresenceStatuses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_MarkTypes_Uuid",
                table: "MarkTypes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Marks_Uuid",
                table: "Marks");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_LessonTypes_Uuid",
                table: "LessonTypes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Lessons_Uuid",
                table: "Lessons");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_LessonPresences_Uuid",
                table: "LessonPresences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks");

            migrationBuilder.DropIndex(
                name: "IX_LessonMarks_LessonId",
                table: "LessonMarks");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Groups_Uuid",
                table: "Groups");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Faculties_Uuid",
                table: "Faculties");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_EmployeePosts_Uuid",
                table: "EmployeePosts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DisciplinesRegisters_Uuid",
                table: "DisciplinesRegisters");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Disciplines_Uuid",
                table: "Disciplines");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Departments_Uuid",
                table: "Departments");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Brigades_Uuid",
                table: "Brigades");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AttestationTypes_Uuid",
                table: "AttestationTypes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Attestations_Uuid",
                table: "Attestations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AttestationMarks_Uuid",
                table: "AttestationMarks");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AcademicYears_Uuid",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "UniversityEmployers");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "TrainingDirections");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "StudentPersons");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "StudentNotes");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Semesters");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "PresenceStatuses");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "MarkTypes");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "LessonTypes");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "LessonPresences");

            migrationBuilder.DropColumn(
                name: "LessonMarkId",
                table: "LessonMarks");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "LessonMarks");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "EmployeePosts");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "DisciplinesRegisters");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Brigades");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "AttestationTypes");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Attestations");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "AttestationMarks");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "AcademicYears");

            migrationBuilder.RenameColumn(
                name: "NotesAboutStudentId",
                table: "StudentNotes",
                newName: "StudentNoteId");

            migrationBuilder.AlterColumn<int>(
                name: "LessonId",
                table: "LessonMarks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes",
                columns: new[] { "DisciplineId", "MarkTypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks",
                columns: new[] { "LessonId", "StudentId", "MarkId" });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedMarkTypes_LessonTypeId",
                table: "SelectedMarkTypes",
                column: "LessonTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonMarks_Lessons_LessonId",
                table: "LessonMarks",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
