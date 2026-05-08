using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MainService.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesOfAttestation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttestationTypes",
                columns: table => new
                {
                    AttestationTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttestationTypes", x => x.AttestationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Attestations",
                columns: table => new
                {
                    AttestationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    AttestationTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attestations", x => x.AttestationId);
                    table.ForeignKey(
                        name: "FK_Attestations_AttestationTypes_AttestationTypeId",
                        column: x => x.AttestationTypeId,
                        principalTable: "AttestationTypes",
                        principalColumn: "AttestationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttestationMarks",
                columns: table => new
                {
                    AttestationMarkId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Mark = table.Column<string>(type: "text", nullable: false),
                    AttestationId = table.Column<int>(type: "integer", nullable: false),
                    AttestationTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttestationMarks", x => x.AttestationMarkId);
                    table.ForeignKey(
                        name: "FK_AttestationMarks_AttestationTypes_AttestationTypeId",
                        column: x => x.AttestationTypeId,
                        principalTable: "AttestationTypes",
                        principalColumn: "AttestationTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttestationMarks_Attestations_AttestationId",
                        column: x => x.AttestationId,
                        principalTable: "Attestations",
                        principalColumn: "AttestationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttestationMarks_AttestationId",
                table: "AttestationMarks",
                column: "AttestationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttestationMarks_AttestationTypeId",
                table: "AttestationMarks",
                column: "AttestationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attestations_AttestationTypeId",
                table: "Attestations",
                column: "AttestationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttestationMarks");

            migrationBuilder.DropTable(
                name: "Attestations");

            migrationBuilder.DropTable(
                name: "AttestationTypes");
        }
    }
}
