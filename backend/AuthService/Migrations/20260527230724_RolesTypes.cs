using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class RolesTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleRights_Roles_RoleId",
                table: "RoleRights");

            migrationBuilder.DropIndex(
                name: "IX_RoleRights_RoleId",
                table: "RoleRights");

            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "RoleRights");

            migrationBuilder.AlterColumn<int>(
                name: "TokenVersion",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "RoleRoleRights",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    RoleRightId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRoleRights", x => new { x.RoleId, x.RoleRightId });
                    table.ForeignKey(
                        name: "FK_RoleRoleRights_RoleRights_RoleRightId",
                        column: x => x.RoleRightId,
                        principalTable: "RoleRights",
                        principalColumn: "RoleRightId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleRoleRights_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolesTypes",
                columns: table => new
                {
                    RoleTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTypes", x => x.RoleTypeId);
                });

            migrationBuilder.CreateTable(
                name: "RolesRolesTypes",
                columns: table => new
                {
                    RoleTypeId = table.Column<int>(type: "integer", nullable: false),
                    RolesRoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesRolesTypes", x => new { x.RoleTypeId, x.RolesRoleId });
                    table.ForeignKey(
                        name: "FK_RolesRolesTypes_RolesTypes_RoleTypeId",
                        column: x => x.RoleTypeId,
                        principalTable: "RolesTypes",
                        principalColumn: "RoleTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolesRolesTypes_Roles_RolesRoleId",
                        column: x => x.RolesRoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleRoleRights_RoleRightId",
                table: "RoleRoleRights",
                column: "RoleRightId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesRolesTypes_RolesRoleId",
                table: "RolesRolesTypes",
                column: "RolesRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleRoleRights");

            migrationBuilder.DropTable(
                name: "RolesRolesTypes");

            migrationBuilder.DropTable(
                name: "RolesTypes");

            migrationBuilder.AlterColumn<long>(
                name: "TokenVersion",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "RoleRights",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleRights_RoleId",
                table: "RoleRights",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRights_Roles_RoleId",
                table: "RoleRights",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
