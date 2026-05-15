using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class TransferFullNameToEmployeeAndStudentPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UniversityEmployers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "UniversityEmployers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "UniversityEmployers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "StudentPersons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "StudentPersons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "StudentPersons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplateForGroup",
                table: "Brigades",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UniversityEmployers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "UniversityEmployers");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "UniversityEmployers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "StudentPersons");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "StudentPersons");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "StudentPersons");

            migrationBuilder.DropColumn(
                name: "IsTemplateForGroup",
                table: "Brigades");

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
        }
    }
}
