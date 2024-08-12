using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audits_AuditTemplates_TemplateId",
                table: "Audits");

            migrationBuilder.DropIndex(
                name: "IX_Audits_TemplateId",
                table: "Audits");

            migrationBuilder.AddColumn<int>(
                name: "AuditId",
                table: "Sections",
                type: "INTEGER",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_Audits_TemplateId",
                table: "Audits",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_AuditTemplates_TemplateId",
                table: "Audits",
                column: "TemplateId",
                principalTable: "AuditTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
