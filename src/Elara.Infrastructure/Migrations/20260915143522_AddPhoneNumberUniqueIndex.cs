using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear duplicate phone numbers (keep only the earliest user's phone)
            migrationBuilder.Sql(@"
                WITH DuplicatePhones AS (
                    SELECT Id, PhoneNumber,
                           ROW_NUMBER() OVER (PARTITION BY PhoneNumber ORDER BY Id) AS RowNum
                    FROM Users
                    WHERE PhoneNumber IS NOT NULL AND PhoneNumber != ''
                )
                UPDATE Users SET PhoneNumber = ''
                FROM Users u
                INNER JOIN DuplicatePhones dp ON u.Id = dp.Id
                WHERE dp.RowNum > 1;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] != ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users");
        }
    }
}
