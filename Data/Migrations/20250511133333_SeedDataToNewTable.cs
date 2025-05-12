using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataToNewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CategoryCompany",
                columns: new[] { "CategoriesId", "CompaniesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CategoryCompany",
                keyColumns: new[] { "CategoriesId", "CompaniesId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "CategoryCompany",
                keyColumns: new[] { "CategoriesId", "CompaniesId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "CategoryCompany",
                keyColumns: new[] { "CategoriesId", "CompaniesId" },
                keyValues: new object[] { 3, 3 });
        }
    }
}
