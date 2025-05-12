using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationBetweenCompanyAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Categories_CategoryId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CategoryId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Companies");

            migrationBuilder.CreateTable(
                name: "CategoryCompany",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    CompaniesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCompany", x => new { x.CategoriesId, x.CompaniesId });
                    table.ForeignKey(
                        name: "FK_CategoryCompany_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryCompany_Companies_CompaniesId",
                        column: x => x.CompaniesId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCompany_CompaniesId",
                table: "CategoryCompany",
                column: "CompaniesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryCompany");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 3);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CategoryId",
                table: "Companies",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Categories_CategoryId",
                table: "Companies",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
