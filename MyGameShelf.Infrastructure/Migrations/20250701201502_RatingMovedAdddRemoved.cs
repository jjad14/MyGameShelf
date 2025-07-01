using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGameShelf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RatingMovedAdddRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Added",
                table: "Games");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "UserGames",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecommended",
                table: "Reviews",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "IsRecommended",
                table: "Reviews");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Reviews",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Added",
                table: "Games",
                type: "int",
                nullable: true);
        }
    }
}
