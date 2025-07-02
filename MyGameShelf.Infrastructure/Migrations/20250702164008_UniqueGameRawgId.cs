using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGameShelf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueGameRawgId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Games_RawgId",
                table: "Games",
                column: "RawgId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Games_RawgId",
                table: "Games");
        }
    }
}
