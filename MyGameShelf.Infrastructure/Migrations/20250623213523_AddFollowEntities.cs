using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGameShelf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeveloperFollows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeveloperId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeveloperId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeveloperFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeveloperFollows_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeveloperFollows_Developers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalTable: "Developers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeveloperFollows_Developers_DeveloperId1",
                        column: x => x.DeveloperId1,
                        principalTable: "Developers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PublisherFollows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublisherId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublisherId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublisherFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublisherFollows_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublisherFollows_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublisherFollows_Publishers_PublisherId1",
                        column: x => x.PublisherId1,
                        principalTable: "Publishers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserFollows",
                columns: table => new
                {
                    FollowerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FolloweeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => new { x.FollowerId, x.FolloweeId });
                    table.ForeignKey(
                        name: "FK_UserFollows_AspNetUsers_FolloweeId",
                        column: x => x.FolloweeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollows_AspNetUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeveloperFollows_DeveloperId",
                table: "DeveloperFollows",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_DeveloperFollows_DeveloperId1",
                table: "DeveloperFollows",
                column: "DeveloperId1");

            migrationBuilder.CreateIndex(
                name: "IX_DeveloperFollows_UserId",
                table: "DeveloperFollows",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PublisherFollows_PublisherId",
                table: "PublisherFollows",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_PublisherFollows_PublisherId1",
                table: "PublisherFollows",
                column: "PublisherId1");

            migrationBuilder.CreateIndex(
                name: "IX_PublisherFollows_UserId",
                table: "PublisherFollows",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_FolloweeId",
                table: "UserFollows",
                column: "FolloweeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeveloperFollows");

            migrationBuilder.DropTable(
                name: "PublisherFollows");

            migrationBuilder.DropTable(
                name: "UserFollows");
        }
    }
}
