using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Laqeeta.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedLostReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Reports",
                columns: new[] { "ReportId", "Category", "Color", "CreatedAt", "Description", "Direction", "Line", "LostAt", "PhoneNumber", "SecretAnswerHash", "SecretQuestion", "Status" },
                values: new object[,]
                {
                    { 1, "Phone", "Black", new DateTime(2026, 10, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Black smartphone with a cracked screen", "Northbound", "Line 1", new DateTime(2026, 10, 30, 9, 0, 0, 0, DateTimeKind.Unspecified), "01000000001", null, null, "Searching" },
                    { 2, "Wallet", "Brown", new DateTime(2026, 10, 30, 13, 0, 0, 0, DateTimeKind.Unspecified), "Brown leather wallet", "Southbound", "Line 1", new DateTime(2026, 10, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), "01000000002", null, null, "Searching" },
                    { 3, "Bag", "Blue", new DateTime(2026, 10, 31, 15, 0, 0, 0, DateTimeKind.Unspecified), "Small blue backpack", "Northbound", "Line 1", new DateTime(2026, 10, 31, 13, 30, 0, 0, DateTimeKind.Unspecified), "01000000003", null, null, "Searching" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "ReportId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "ReportId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reports",
                keyColumn: "ReportId",
                keyValue: 3);
        }
    }
}
