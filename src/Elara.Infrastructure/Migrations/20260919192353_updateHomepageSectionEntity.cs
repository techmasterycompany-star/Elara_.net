using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateHomepageSectionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "HomepageSections",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomepageSections_CategoryId",
                table: "HomepageSections",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomepageSections_Categories_CategoryId",
                table: "HomepageSections",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomepageSections_Categories_CategoryId",
                table: "HomepageSections");

            migrationBuilder.DropIndex(
                name: "IX_HomepageSections_CategoryId",
                table: "HomepageSections");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "HomepageSections");
        }
    }
}
