using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SellerApplications_UserId",
                table: "SellerApplications");

            migrationBuilder.CreateIndex(
                name: "IX_SellerApplications_UserId",
                table: "SellerApplications",
                column: "UserId",
                unique: true,
                filter: "[Status] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SellerApplications_UserId",
                table: "SellerApplications");

            migrationBuilder.CreateIndex(
                name: "IX_SellerApplications_UserId",
                table: "SellerApplications",
                column: "UserId");
        }
    }
}
