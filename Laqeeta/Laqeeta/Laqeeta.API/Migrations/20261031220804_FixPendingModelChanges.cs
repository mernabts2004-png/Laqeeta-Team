using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laqeeta.API.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecretAnswerHash",
                table: "Reports",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretQuestion",
                table: "Reports",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecretAnswerHash",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "SecretQuestion",
                table: "Reports");
        }
    }
}
