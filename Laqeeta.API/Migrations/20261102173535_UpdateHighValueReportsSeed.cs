using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laqeeta.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHighValueReportsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "ReportId",
                keyValue: 1,
                columns: new[] { "SecretAnswerHash", "SecretQuestion" },
                values: new object[] { "AQAAAAIAAYagAAAAEL9Qzq3sZ4bR+FlqdBiB+saZYjE+h1GXZT2euYkgNK6cN0E4aMjvgMsaGbEM39K8+Q==", "What was the phone wallpaper?" });

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "ReportId",
                keyValue: 2,
                columns: new[] { "SecretAnswerHash", "SecretQuestion" },
                values: new object[] { "AQAAAAIAAYagAAAAEH69+5l/u6b5owCePp8w+rCQkGDG8YI43GxSFKoCXlMKClT1NhAss4yZjcnX+sDdtQ==", "What is one item inside the wallet?" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "ReportId",
                keyValue: 1,
                columns: new[] { "SecretAnswerHash", "SecretQuestion" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Reports",
                keyColumn: "ReportId",
                keyValue: 2,
                columns: new[] { "SecretAnswerHash", "SecretQuestion" },
                values: new object[] { null, null });
        }
    }
}
