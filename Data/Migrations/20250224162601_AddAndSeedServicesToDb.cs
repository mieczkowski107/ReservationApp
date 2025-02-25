using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAndSeedServicesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationMinutes = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsPrepaymentRequired = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CompanyId", "Description", "DurationMinutes", "IsPrepaymentRequired", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Tattoo Service Description", new TimeSpan(0, 0, 30, 0, 0), true, "Tattoo Service 1", 100m },
                    { 2, 1, "Tattoo Service Description", new TimeSpan(0, 0, 30, 0, 0), true, "Tattoo Service 2", 200m },
                    { 3, 1, "Tattoo Service Description", new TimeSpan(0, 0, 30, 0, 0), true, "Tattoo Service 2", 200m },
                    { 4, 2, "Barber Service Description", new TimeSpan(0, 0, 30, 0, 0), true, "Barber Service 1", 50m },
                    { 5, 2, "Barber Service Description", new TimeSpan(0, 0, 45, 0, 0), true, "Barber Service 2", 75m },
                    { 6, 3, "Physiotherapy Service Description", new TimeSpan(0, 1, 0, 0, 0), true, "Physiotherapy Service 1", 150m },
                    { 7, 3, "Physiotherapy Service Description", new TimeSpan(0, 0, 45, 0, 0), true, "Physiotherapy Service 2", 200m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_CompanyId",
                table: "Services",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
