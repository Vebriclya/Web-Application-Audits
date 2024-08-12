using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditAddName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuditName",
                table: "Audits",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditName",
                table: "Audits");
        }
    }
}
