using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditQuestion_AuditSection_AuditSectionId",
                table: "AuditQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditSection_Audits_AuditId",
                table: "AuditSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditSection",
                table: "AuditSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditQuestion",
                table: "AuditQuestion");

            migrationBuilder.RenameTable(
                name: "AuditSection",
                newName: "AuditSections");

            migrationBuilder.RenameTable(
                name: "AuditQuestion",
                newName: "AuditQuestions");

            migrationBuilder.RenameIndex(
                name: "IX_AuditSection_AuditId",
                table: "AuditSections",
                newName: "IX_AuditSections_AuditId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditQuestion_AuditSectionId",
                table: "AuditQuestions",
                newName: "IX_AuditQuestions_AuditSectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditSections",
                table: "AuditSections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditQuestions",
                table: "AuditQuestions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditQuestions_AuditSections_AuditSectionId",
                table: "AuditQuestions",
                column: "AuditSectionId",
                principalTable: "AuditSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditSections_Audits_AuditId",
                table: "AuditSections",
                column: "AuditId",
                principalTable: "Audits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditQuestions_AuditSections_AuditSectionId",
                table: "AuditQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditSections_Audits_AuditId",
                table: "AuditSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditSections",
                table: "AuditSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditQuestions",
                table: "AuditQuestions");

            migrationBuilder.RenameTable(
                name: "AuditSections",
                newName: "AuditSection");

            migrationBuilder.RenameTable(
                name: "AuditQuestions",
                newName: "AuditQuestion");

            migrationBuilder.RenameIndex(
                name: "IX_AuditSections_AuditId",
                table: "AuditSection",
                newName: "IX_AuditSection_AuditId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditQuestions_AuditSectionId",
                table: "AuditQuestion",
                newName: "IX_AuditQuestion_AuditSectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditSection",
                table: "AuditSection",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditQuestion",
                table: "AuditQuestion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditQuestion_AuditSection_AuditSectionId",
                table: "AuditQuestion",
                column: "AuditSectionId",
                principalTable: "AuditSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditSection_Audits_AuditId",
                table: "AuditSection",
                column: "AuditId",
                principalTable: "Audits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
