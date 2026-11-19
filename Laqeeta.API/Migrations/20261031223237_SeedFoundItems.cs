using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Laqeeta.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedFoundItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "Category", "Color", "Description", "Direction", "FoundAt", "FoundAtStationId", "Line", "MovedToCentralAt", "Status", "StorageLocation" },
                values: new object[,]
                {
                    { 1, "Phone", "Black", "Black smartphone with a cracked screen", "Northbound", new DateTime(2026, 10, 30, 9, 15, 0, 0, DateTimeKind.Unspecified), 1, "Line 1", null, "Found", "Station" },
                    { 2, "Wallet", "Brown", "Brown leather wallet", "Southbound", new DateTime(2026, 10, 30, 12, 30, 0, 0, DateTimeKind.Unspecified), 2, "Line 1", null, "Found", "Station" },
                    { 3, "Bag", "Blue", "Small blue backpack", "Northbound", new DateTime(2026, 10, 31, 14, 0, 0, 0, DateTimeKind.Unspecified), 3, "Line 1", null, "Found", "Station" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 3);
        }
    }
}
