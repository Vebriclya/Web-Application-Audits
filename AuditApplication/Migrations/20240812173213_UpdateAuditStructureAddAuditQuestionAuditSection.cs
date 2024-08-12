using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditStructureAddAuditQuestionAuditSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Audits_AuditId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_AuditId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "AuditId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Audits");

            migrationBuilder.CreateTable(
                name: "AuditSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    AuditId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditSection_Audits_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    AuditSectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditQuestion_AuditSection_AuditSectionId",
                        column: x => x.AuditSectionId,
                        principalTable: "AuditSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditQuestion_AuditSectionId",
                table: "AuditQuestion",
                column: "AuditSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditSection_AuditId",
                table: "AuditSection",
                column: "AuditId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditQuestion");

            migrationBuilder.DropTable(
                name: "AuditSection");

            migrationBuilder.AddColumn<int>(
                name: "AuditId",
                table: "Sections",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "Audits",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_AuditId",
                table: "Sections",
                column: "AuditId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Audits_AuditId",
                table: "Sections",
                column: "AuditId",
                principalTable: "Audits",
                principalColumn: "Id");
        }
    }
}
