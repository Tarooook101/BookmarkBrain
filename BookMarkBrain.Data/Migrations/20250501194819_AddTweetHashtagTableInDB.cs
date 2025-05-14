using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarkBrain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTweetHashtagTableInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TweetHashtags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TweetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashtagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashtagId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TweetId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TweetHashtags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TweetHashtags_Hashtags_HashtagId",
                        column: x => x.HashtagId,
                        principalTable: "Hashtags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TweetHashtags_Hashtags_HashtagId1",
                        column: x => x.HashtagId1,
                        principalTable: "Hashtags",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TweetHashtags_Tweets_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TweetHashtags_Tweets_TweetId1",
                        column: x => x.TweetId1,
                        principalTable: "Tweets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TweetHashtags_HashtagId",
                table: "TweetHashtags",
                column: "HashtagId");

            migrationBuilder.CreateIndex(
                name: "IX_TweetHashtags_HashtagId1",
                table: "TweetHashtags",
                column: "HashtagId1");

            migrationBuilder.CreateIndex(
                name: "IX_TweetHashtags_TweetId_HashtagId",
                table: "TweetHashtags",
                columns: new[] { "TweetId", "HashtagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TweetHashtags_TweetId1",
                table: "TweetHashtags",
                column: "TweetId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TweetHashtags");
        }
    }
}
