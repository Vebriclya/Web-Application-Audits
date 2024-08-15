using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditApplication.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditIdFromQuestionResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_Audits_AuditId",
                table: "QuestionResponses");

            migrationBuilder.AlterColumn<int>(
                name: "AuditId",
                table: "QuestionResponses",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Audits_AuditId",
                table: "QuestionResponses",
                column: "AuditId",
                principalTable: "Audits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_Audits_AuditId",
                table: "QuestionResponses");

            migrationBuilder.AlterColumn<int>(
                name: "AuditId",
                table: "QuestionResponses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Audits_AuditId",
                table: "QuestionResponses",
                column: "AuditId",
                principalTable: "Audits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
