using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionsQuestionsAndResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audits_AuditTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "AuditTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AuditTemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_AuditTemplates_AuditTemplateId",
                        column: x => x.AuditTemplateId,
                        principalTable: "AuditTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    SectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuditId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RadioAnswer = table.Column<int>(type: "INTEGER", nullable: false),
                    TextAnswer = table.Column<string>(type: "TEXT", nullable: false),
                    AttachmentPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionResponses_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionResponses_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audits_TemplateId",
                table: "Audits",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_AuditId",
                table: "QuestionResponses",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_QuestionId",
                table: "QuestionResponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SectionId",
                table: "Questions",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_AuditTemplateId",
                table: "Sections",
                column: "AuditTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionResponses");

            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "AuditTemplates");
        }
    }
}
