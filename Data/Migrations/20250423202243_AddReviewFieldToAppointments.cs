using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewFieldToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_AppointmentId",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AppointmentId",
                table: "Review",
                column: "AppointmentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_AppointmentId",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AppointmentId",
                table: "Review",
                column: "AppointmentId");
        }
    }
}
