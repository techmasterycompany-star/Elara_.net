using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestSessionIdToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuestSessionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestSessionId",
                table: "Orders");
        }
    }
}
