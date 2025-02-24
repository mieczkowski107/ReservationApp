using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class seedCompanieData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "CategoryId", "City", "Email", "Name", "Phone", "State", "Zip" },
                values: new object[,]
                {
                    { 1, "1234 Tattoo St", 1, "Tattoo City", "ink@gmail.com", "Ink Master", "123-456-7890", "Tattoo State", "12345" },
                    { 2, "1234 Barber St", 2, "Barber City", "barber@gmail.com", "Barber Shop", "123-456-7890", "Barber State", "12345" },
                    { 3, "1234 Physiotherapy St", 3, "Physiotherapy City", "physio@gmail.com", "Physiotherapy Clinic", "123-456-7890", "Physiotherapy State", "12345" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
