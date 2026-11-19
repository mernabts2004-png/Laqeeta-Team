using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laqeeta.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Line = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LostAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                    table.CheckConstraint("CK_Reports_Status", "[Status] IN (N'Searching', N'PossibleMatch', N'ReadyToCollect', N'Closed')");
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    StationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.StationId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Line = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FoundAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FoundAtStationId = table.Column<int>(type: "int", nullable: false),
                    StorageLocation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MovedToCentralAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.CheckConstraint("CK_Items_CentralStorageDate", "([StorageLocation] = N'Station' AND [MovedToCentralAt] IS NULL) OR ([StorageLocation] = N'CentralStoreroom' AND [MovedToCentralAt] IS NOT NULL)");
                    table.CheckConstraint("CK_Items_Status", "[Status] IN (N'Found', N'Reserved', N'Returned')");
                    table.CheckConstraint("CK_Items_StorageLocation", "[StorageLocation] IN (N'Station', N'CentralStoreroom')");
                    table.ForeignKey(
                        name: "FK_Items_Stations_FoundAtStationId",
                        column: x => x.FoundAtStationId,
                        principalTable: "Stations",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    ClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdVerified = table.Column<bool>(type: "bit", nullable: false),
                    SecretQuestionVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HandedOverAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.ClaimId);
                    table.CheckConstraint("CK_Claims_CompletedDate", "([Status] = N'Completed' AND [HandedOverAt] IS NOT NULL) OR ([Status] <> N'Completed' AND [HandedOverAt] IS NULL)");
                    table.CheckConstraint("CK_Claims_Status", "[Status] IN (N'Pending', N'Rejected', N'Approved', N'Completed')");
                    table.ForeignKey(
                        name: "FK_Claims_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "ReportId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ItemId",
                table: "Claims",
                column: "ItemId",
                unique: true,
                filter: "[Status] = N'Completed'");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ReportId_ItemId",
                table: "Claims",
                columns: new[] { "ReportId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Category_Line_FoundAt",
                table: "Items",
                columns: new[] { "Category", "Line", "FoundAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_FoundAtStationId",
                table: "Items",
                column: "FoundAtStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Category_Line_LostAt",
                table: "Reports",
                columns: new[] { "Category", "Line", "LostAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Stations_Name",
                table: "Stations",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
