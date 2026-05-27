using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedMarkTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks");

            migrationBuilder.DropIndex(
                name: "IX_LessonMarks_LessonId",
                table: "LessonMarks");

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

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "Lessons",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Lessons",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "LessonMarkId",
                table: "LessonMarks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "DisciplineTypeId",
                table: "Disciplines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes",
                columns: new[] { "LessonTypeId", "MarkTypeId", "DisciplineTypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks",
                column: "LessonMarkId");

            migrationBuilder.CreateTable(
                name: "DisciplinesTypes",
                columns: table => new
                {
                    DisciplineTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinesTypes", x => x.DisciplineTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedMarkTypes_DisciplineTypeId",
                table: "SelectedMarkTypes",
                column: "DisciplineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonMarks_LessonId_MarkId_StudentId",
                table: "LessonMarks",
                columns: new[] { "LessonId", "MarkId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_DisciplineTypeId",
                table: "Disciplines",
                column: "DisciplineTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_DisciplinesTypes_DisciplineTypeId",
                table: "Disciplines",
                column: "DisciplineTypeId",
                principalTable: "DisciplinesTypes",
                principalColumn: "DisciplineTypeId",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_DisciplinesTypes_DisciplineTypeId",
                table: "Disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_DisciplinesTypes_DisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropTable(
                name: "DisciplinesTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes");

            migrationBuilder.DropIndex(
                name: "IX_SelectedMarkTypes_DisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks");

            migrationBuilder.DropIndex(
                name: "IX_LessonMarks_LessonId_MarkId_StudentId",
                table: "LessonMarks");

            migrationBuilder.DropIndex(
                name: "IX_Disciplines_DisciplineTypeId",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "DisciplineTypeId",
                table: "SelectedMarkTypes");

            migrationBuilder.DropColumn(
                name: "DisciplineTypeId",
                table: "Disciplines");

            migrationBuilder.AlterColumn<int>(
                name: "DisciplineId",
                table: "SelectedMarkTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LessonMarkId",
                table: "LessonMarks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedMarkTypes",
                table: "SelectedMarkTypes",
                columns: new[] { "LessonTypeId", "MarkTypeId", "DisciplineId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonMarks",
                table: "LessonMarks",
                columns: new[] { "LessonMarkId", "MarkId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonMarks_LessonId",
                table: "LessonMarks",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedMarkTypes_Disciplines_DisciplineId",
                table: "SelectedMarkTypes",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "DisciplineId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
