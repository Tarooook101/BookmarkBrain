using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarkBrain.Data.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TweetHashtags_Hashtags_HashtagId1",
                table: "TweetHashtags");

            migrationBuilder.DropForeignKey(
                name: "FK_TweetHashtags_Tweets_TweetId1",
                table: "TweetHashtags");

            migrationBuilder.DropIndex(
                name: "IX_TweetHashtags_HashtagId1",
                table: "TweetHashtags");

            migrationBuilder.DropIndex(
                name: "IX_TweetHashtags_TweetId1",
                table: "TweetHashtags");

            migrationBuilder.DropColumn(
                name: "HashtagId1",
                table: "TweetHashtags");

            migrationBuilder.DropColumn(
                name: "TweetId1",
                table: "TweetHashtags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HashtagId1",
                table: "TweetHashtags",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TweetId1",
                table: "TweetHashtags",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TweetHashtags_HashtagId1",
                table: "TweetHashtags",
                column: "HashtagId1");

            migrationBuilder.CreateIndex(
                name: "IX_TweetHashtags_TweetId1",
                table: "TweetHashtags",
                column: "TweetId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TweetHashtags_Hashtags_HashtagId1",
                table: "TweetHashtags",
                column: "HashtagId1",
                principalTable: "Hashtags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TweetHashtags_Tweets_TweetId1",
                table: "TweetHashtags",
                column: "TweetId1",
                principalTable: "Tweets",
                principalColumn: "Id");
        }
    }
}
