using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipBetweenAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttestationMarks_Attestations_AttestationId",
                table: "AttestationMarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Brigades_Groups_GroupsGroupId",
                table: "Brigades");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Faculties_FacultiesFacultyId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_Brigades_BrigadesBrigadeId",
                table: "Disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_TrainingDirections_TrainingDirectionId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPresences_PresenceStatuses_PresenceStatusId",
                table: "LessonPresences");

            migrationBuilder.DropForeignKey(
                name: "FK_Marks_MarkTypes_MarkTypeId",
                table: "Marks");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Professors_UniversityEmployerId",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_Marks_MarkTypeId",
                table: "Marks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks");

            migrationBuilder.DropIndex(
                name: "IX_LessonMarks_LessonId",
                table: "LessonMarks");

            migrationBuilder.DropIndex(
                name: "IX_Disciplines_BrigadesBrigadeId",
                table: "Disciplines");

            migrationBuilder.DropIndex(
                name: "IX_Departments_FacultiesFacultyId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Brigades_GroupsGroupId",
                table: "Brigades");

            migrationBuilder.DropIndex(
                name: "IX_AttestationMarks_AttestationId",
                table: "AttestationMarks");

            migrationBuilder.DropColumn(
                name: "LessonMarkId",
                table: "LessonMarks");

            migrationBuilder.DropColumn(
                name: "BrigadesBrigadeId",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "FacultiesFacultyId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "GroupsGroupId",
                table: "Brigades");

            migrationBuilder.DropColumn(
                name: "AttestationId",
                table: "AttestationMarks");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentPersonId",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisciplinesDisciplineId",
                table: "StudentNotes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "Professors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UniversityEmployerId",
                table: "MarkTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MarkTypeId",
                table: "Marks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisciplineId",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LessonTypeId",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PresenceStatusId",
                table: "LessonPresences",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrainingDirectionId",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "Disciplines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisciplineRegisterId",
                table: "Disciplines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Disciplines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Disciplines",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "Departments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "Brigades",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "AttestationMarkId",
                table: "Attestations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisciplineId",
                table: "Attestations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Attestations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks",
                columns: new[] { "LessonId", "StudentId", "MarkId" });

            migrationBuilder.CreateTable(
                name: "BrigadesDisciplines",
                columns: table => new
                {
                    BrigadesBrigadeId = table.Column<int>(type: "integer", nullable: false),
                    DisciplinesDisciplineId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrigadesDisciplines", x => new { x.BrigadesBrigadeId, x.DisciplinesDisciplineId });
                    table.ForeignKey(
                        name: "FK_BrigadesDisciplines_Brigades_BrigadesBrigadeId",
                        column: x => x.BrigadesBrigadeId,
                        principalTable: "Brigades",
                        principalColumn: "BrigadeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrigadesDisciplines_Disciplines_DisciplinesDisciplineId",
                        column: x => x.DisciplinesDisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "DisciplineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Curators",
                columns: table => new
                {
                    ProfessorId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curators", x => new { x.ProfessorId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_Curators_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Curators_Professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professors",
                        principalColumn: "ProfessorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinesGroups",
                columns: table => new
                {
                    DisciplineId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinesGroups", x => new { x.DisciplineId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_DisciplinesGroups_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "DisciplineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisciplinesGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinesProfessors",
                columns: table => new
                {
                    DisciplinesDisciplineId = table.Column<int>(type: "integer", nullable: false),
                    ProfessorsProfessorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinesProfessors", x => new { x.DisciplinesDisciplineId, x.ProfessorsProfessorId });
                    table.ForeignKey(
                        name: "FK_DisciplinesProfessors_Disciplines_DisciplinesDisciplineId",
                        column: x => x.DisciplinesDisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "DisciplineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisciplinesProfessors_Professors_ProfessorsProfessorId",
                        column: x => x.ProfessorsProfessorId,
                        principalTable: "Professors",
                        principalColumn: "ProfessorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonTypes",
                columns: table => new
                {
                    LessonTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonTypes", x => x.LessonTypeId);
                });

            migrationBuilder.CreateTable(
                name: "StudentPersons",
                columns: table => new
                {
                    StudentPersonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPersons", x => x.StudentPersonId);
                    table.ForeignKey(
                        name: "FK_StudentPersons_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedMarkTypes",
                columns: table => new
                {
                    MarkTypeId = table.Column<int>(type: "integer", nullable: false),
                    DisciplineId = table.Column<int>(type: "integer", nullable: false),
                    LessonTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedMarkTypes", x => new { x.DisciplineId, x.MarkTypeId });
                    table.ForeignKey(
                        name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "DisciplineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedMarkTypes_LessonTypes_LessonTypeId",
                        column: x => x.LessonTypeId,
                        principalTable: "LessonTypes",
                        principalColumn: "LessonTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedMarkTypes_MarkTypes_MarkTypeId",
                        column: x => x.MarkTypeId,
                        principalTable: "MarkTypes",
                        principalColumn: "MarkTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentPersonId",
                table: "Students",
                column: "StudentPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotes_DisciplinesDisciplineId",
                table: "StudentNotes",
                column: "DisciplinesDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_AcademicYearId",
                table: "Professors",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_UniversityEmployerId",
                table: "Professors",
                column: "UniversityEmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarkTypes_UniversityEmployerId",
                table: "MarkTypes",
                column: "UniversityEmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_MarkTypeId_Value",
                table: "Marks",
                columns: new[] { "MarkTypeId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_DisciplineId",
                table: "Lessons",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LessonTypeId",
                table: "Lessons",
                column: "LessonTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_FacultyId",
                table: "Groups",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_AcademicYearId",
                table: "Disciplines",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_DisciplineRegisterId",
                table: "Disciplines",
                column: "DisciplineRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_SemesterId",
                table: "Disciplines",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FacultyId",
                table: "Departments",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Brigades_GroupId",
                table: "Brigades",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Attestations_AttestationMarkId",
                table: "Attestations",
                column: "AttestationMarkId");

            migrationBuilder.CreateIndex(
                name: "IX_Attestations_DisciplineId",
                table: "Attestations",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Attestations_StudentId",
                table: "Attestations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_BrigadesDisciplines_DisciplinesDisciplineId",
                table: "BrigadesDisciplines",
                column: "DisciplinesDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Curators_GroupId",
                table: "Curators",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinesGroups_GroupId",
                table: "DisciplinesGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinesProfessors_ProfessorsProfessorId",
                table: "DisciplinesProfessors",
                column: "ProfessorsProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedMarkTypes_LessonTypeId",
                table: "SelectedMarkTypes",
                column: "LessonTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedMarkTypes_MarkTypeId",
                table: "SelectedMarkTypes",
                column: "MarkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPersons_UserId",
                table: "StudentPersons",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attestations_AttestationMarks_AttestationMarkId",
                table: "Attestations",
                column: "AttestationMarkId",
                principalTable: "AttestationMarks",
                principalColumn: "AttestationMarkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attestations_Disciplines_DisciplineId",
                table: "Attestations",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "DisciplineId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attestations_Students_StudentId",
                table: "Attestations",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Brigades_Groups_GroupId",
                table: "Brigades",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                table: "Departments",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "FacultyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_AcademicYears_AcademicYearId",
                table: "Disciplines",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "AcademicYearId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_DisciplinesRegisters_DisciplineRegisterId",
                table: "Disciplines",
                column: "DisciplineRegisterId",
                principalTable: "DisciplinesRegisters",
                principalColumn: "DisciplineId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_Semesters_SemesterId",
                table: "Disciplines",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "SemesterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Faculties_FacultyId",
                table: "Groups",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "FacultyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_TrainingDirections_TrainingDirectionId",
                table: "Groups",
                column: "TrainingDirectionId",
                principalTable: "TrainingDirections",
                principalColumn: "TrainingDirectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPresences_PresenceStatuses_PresenceStatusId",
                table: "LessonPresences",
                column: "PresenceStatusId",
                principalTable: "PresenceStatuses",
                principalColumn: "PresenceStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Disciplines_DisciplineId",
                table: "Lessons",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "DisciplineId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LessonTypes_LessonTypeId",
                table: "Lessons",
                column: "LessonTypeId",
                principalTable: "LessonTypes",
                principalColumn: "LessonTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_MarkTypes_MarkTypeId",
                table: "Marks",
                column: "MarkTypeId",
                principalTable: "MarkTypes",
                principalColumn: "MarkTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarkTypes_UniversityEmployers_UniversityEmployerId",
                table: "MarkTypes",
                column: "UniversityEmployerId",
                principalTable: "UniversityEmployers",
                principalColumn: "UniversityEmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_AcademicYears_AcademicYearId",
                table: "Professors",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "AcademicYearId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNotes_Disciplines_DisciplinesDisciplineId",
                table: "StudentNotes",
                column: "DisciplinesDisciplineId",
                principalTable: "Disciplines",
                principalColumn: "DisciplineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentPersons_StudentPersonId",
                table: "Students",
                column: "StudentPersonId",
                principalTable: "StudentPersons",
                principalColumn: "StudentPersonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attestations_AttestationMarks_AttestationMarkId",
                table: "Attestations");

            migrationBuilder.DropForeignKey(
                name: "FK_Attestations_Disciplines_DisciplineId",
                table: "Attestations");

            migrationBuilder.DropForeignKey(
                name: "FK_Attestations_Students_StudentId",
                table: "Attestations");

            migrationBuilder.DropForeignKey(
                name: "FK_Brigades_Groups_GroupId",
                table: "Brigades");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Faculties_FacultyId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_AcademicYears_AcademicYearId",
                table: "Disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_DisciplinesRegisters_DisciplineRegisterId",
                table: "Disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_Semesters_SemesterId",
                table: "Disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Faculties_FacultyId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_TrainingDirections_TrainingDirectionId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonPresences_PresenceStatuses_PresenceStatusId",
                table: "LessonPresences");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Disciplines_DisciplineId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LessonTypes_LessonTypeId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Marks_MarkTypes_MarkTypeId",
                table: "Marks");

            migrationBuilder.DropForeignKey(
                name: "FK_MarkTypes_UniversityEmployers_UniversityEmployerId",
                table: "MarkTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Professors_AcademicYears_AcademicYearId",
                table: "Professors");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentNotes_Disciplines_DisciplinesDisciplineId",
                table: "StudentNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentPersons_StudentPersonId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "BrigadesDisciplines");

            migrationBuilder.DropTable(
                name: "Curators");

            migrationBuilder.DropTable(
                name: "DisciplinesGroups");

            migrationBuilder.DropTable(
                name: "DisciplinesProfessors");

            migrationBuilder.DropTable(
                name: "SelectedMarkTypes");

            migrationBuilder.DropTable(
                name: "StudentPersons");

            migrationBuilder.DropTable(
                name: "LessonTypes");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentPersonId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_StudentNotes_DisciplinesDisciplineId",
                table: "StudentNotes");

            migrationBuilder.DropIndex(
                name: "IX_Professors_AcademicYearId",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_Professors_UniversityEmployerId",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_MarkTypes_UniversityEmployerId",
                table: "MarkTypes");

            migrationBuilder.DropIndex(
                name: "IX_Marks_MarkTypeId_Value",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_DisciplineId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_LessonTypeId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks");

            migrationBuilder.DropIndex(
                name: "IX_Groups_FacultyId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Disciplines_AcademicYearId",
                table: "Disciplines");

            migrationBuilder.DropIndex(
                name: "IX_Disciplines_DisciplineRegisterId",
                table: "Disciplines");

            migrationBuilder.DropIndex(
                name: "IX_Disciplines_SemesterId",
                table: "Disciplines");

            migrationBuilder.DropIndex(
                name: "IX_Departments_FacultyId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Brigades_GroupId",
                table: "Brigades");

            migrationBuilder.DropIndex(
                name: "IX_Attestations_AttestationMarkId",
                table: "Attestations");

            migrationBuilder.DropIndex(
                name: "IX_Attestations_DisciplineId",
                table: "Attestations");

            migrationBuilder.DropIndex(
                name: "IX_Attestations_StudentId",
                table: "Attestations");

            migrationBuilder.DropColumn(
                name: "StudentPersonId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DisciplinesDisciplineId",
                table: "StudentNotes");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "UniversityEmployerId",
                table: "MarkTypes");

            migrationBuilder.DropColumn(
                name: "DisciplineId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "LessonTypeId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "DisciplineRegisterId",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "AttestationMarkId",
                table: "Attestations");

            migrationBuilder.DropColumn(
                name: "DisciplineId",
                table: "Attestations");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Attestations");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "Students",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MarkTypeId",
                table: "Marks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "PresenceStatusId",
                table: "LessonPresences",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "LessonMarkId",
                table: "LessonMarks",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "TrainingDirectionId",
                table: "Groups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "BrigadesBrigadeId",
                table: "Disciplines",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacultiesFacultyId",
                table: "Departments",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "Brigades",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupsGroupId",
                table: "Brigades",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttestationId",
                table: "AttestationMarks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks",
                column: "LessonMarkId");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_UniversityEmployerId",
                table: "Professors",
                column: "UniversityEmployerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marks_MarkTypeId",
                table: "Marks",
                column: "MarkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonMarks_LessonId",
                table: "LessonMarks",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_BrigadesBrigadeId",
                table: "Disciplines",
                column: "BrigadesBrigadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_FacultiesFacultyId",
                table: "Departments",
                column: "FacultiesFacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Brigades_GroupsGroupId",
                table: "Brigades",
                column: "GroupsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AttestationMarks_AttestationId",
                table: "AttestationMarks",
                column: "AttestationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttestationMarks_Attestations_AttestationId",
                table: "AttestationMarks",
                column: "AttestationId",
                principalTable: "Attestations",
                principalColumn: "AttestationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Brigades_Groups_GroupsGroupId",
                table: "Brigades",
                column: "GroupsGroupId",
                principalTable: "Groups",
                principalColumn: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Faculties_FacultiesFacultyId",
                table: "Departments",
                column: "FacultiesFacultyId",
                principalTable: "Faculties",
                principalColumn: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_Brigades_BrigadesBrigadeId",
                table: "Disciplines",
                column: "BrigadesBrigadeId",
                principalTable: "Brigades",
                principalColumn: "BrigadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_TrainingDirections_TrainingDirectionId",
                table: "Groups",
                column: "TrainingDirectionId",
                principalTable: "TrainingDirections",
                principalColumn: "TrainingDirectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonPresences_PresenceStatuses_PresenceStatusId",
                table: "LessonPresences",
                column: "PresenceStatusId",
                principalTable: "PresenceStatuses",
                principalColumn: "PresenceStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_MarkTypes_MarkTypeId",
                table: "Marks",
                column: "MarkTypeId",
                principalTable: "MarkTypes",
                principalColumn: "MarkTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId");
        }
    }
}
