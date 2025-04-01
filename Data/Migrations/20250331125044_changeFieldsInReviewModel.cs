using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class changeFieldsInReviewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_AspNetUsers_UserId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Services_ServiceId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_UserId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Review",
                newName: "AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_ServiceId",
                table: "Review",
                newName: "IX_Review_AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Appointments_AppointmentId",
                table: "Review",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Appointments_AppointmentId",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "Review",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_AppointmentId",
                table: "Review",
                newName: "IX_Review_ServiceId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Review",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_AspNetUsers_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Services_ServiceId",
                table: "Review",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
