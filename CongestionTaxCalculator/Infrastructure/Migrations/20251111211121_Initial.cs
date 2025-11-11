using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CongestionTaxCalculator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxDailyFee = table.Column<int>(type: "int", nullable: false),
                    SingleChargeIntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    IsJulyTollFree = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TollFeeSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Fee = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TollFeeSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TollFeeSchedules_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TollFreeDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    IsDayBeforeHoliday = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TollFreeDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TollFreeDates_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "IsJulyTollFree", "MaxDailyFee", "Name", "SingleChargeIntervalMinutes" },
                values: new object[] { 1, true, 60, "Gothenburg", 60 });

            migrationBuilder.InsertData(
                table: "TollFeeSchedules",
                columns: new[] { "Id", "CityId", "EndTime", "Fee", "StartTime" },
                values: new object[,]
                {
                    { 1, 1, new TimeOnly(6, 29, 0), 8, new TimeOnly(6, 0, 0) },
                    { 2, 1, new TimeOnly(6, 59, 0), 13, new TimeOnly(6, 30, 0) },
                    { 3, 1, new TimeOnly(7, 59, 0), 18, new TimeOnly(7, 0, 0) },
                    { 4, 1, new TimeOnly(8, 29, 0), 13, new TimeOnly(8, 0, 0) },
                    { 5, 1, new TimeOnly(14, 59, 0), 8, new TimeOnly(8, 30, 0) },
                    { 6, 1, new TimeOnly(15, 29, 0), 13, new TimeOnly(15, 0, 0) },
                    { 7, 1, new TimeOnly(16, 59, 0), 18, new TimeOnly(15, 30, 0) },
                    { 8, 1, new TimeOnly(17, 59, 0), 13, new TimeOnly(17, 0, 0) },
                    { 9, 1, new TimeOnly(18, 29, 0), 8, new TimeOnly(18, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "TollFreeDates",
                columns: new[] { "Id", "CityId", "Day", "Description", "IsDayBeforeHoliday", "Month", "Year" },
                values: new object[,]
                {
                    { 1, 1, 1, "New Year's Day", false, 1, 2013 },
                    { 2, 1, 28, "Maundy Thursday", false, 3, 2013 },
                    { 3, 1, 29, "Good Friday", false, 3, 2013 },
                    { 4, 1, 1, "Easter Monday", false, 4, 2013 },
                    { 5, 1, 30, "Day before May 1st", false, 4, 2013 },
                    { 6, 1, 1, "May Day", false, 5, 2013 },
                    { 7, 1, 8, "Ascension Day Eve", false, 5, 2013 },
                    { 8, 1, 9, "Ascension Day", false, 5, 2013 },
                    { 9, 1, 5, "National Day Eve", false, 6, 2013 },
                    { 10, 1, 6, "National Day", false, 6, 2013 },
                    { 11, 1, 21, "Midsummer Eve", false, 6, 2013 },
                    { 12, 1, 1, "All Saints' Day", false, 11, 2013 },
                    { 13, 1, 24, "Christmas Eve", false, 12, 2013 },
                    { 14, 1, 25, "Christmas Day", false, 12, 2013 },
                    { 15, 1, 26, "Boxing Day", false, 12, 2013 },
                    { 16, 1, 31, "New Year's Eve", false, 12, 2013 },
                    { 17, 1, 31, "Day before New Year", true, 12, 2012 },
                    { 18, 1, 27, "Day before Maundy Thursday", true, 3, 2013 },
                    { 19, 1, 31, "Day before Easter Monday", true, 3, 2013 },
                    { 20, 1, 29, "Day before April 30", true, 4, 2013 },
                    { 21, 1, 7, "Day before May 8", true, 5, 2013 },
                    { 22, 1, 4, "Day before National Day Eve", true, 6, 2013 },
                    { 23, 1, 20, "Day before Midsummer Eve", true, 6, 2013 },
                    { 24, 1, 31, "Day before All Saints", true, 10, 2013 },
                    { 25, 1, 23, "Day before Christmas Eve", true, 12, 2013 },
                    { 26, 1, 30, "Day before New Year's Eve", true, 12, 2013 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TollFeeSchedules_CityId",
                table: "TollFeeSchedules",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_TollFreeDates_CityId",
                table: "TollFreeDates",
                column: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TollFeeSchedules");

            migrationBuilder.DropTable(
                name: "TollFreeDates");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
