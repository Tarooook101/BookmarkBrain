using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMarkBrain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTweetTableInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tweets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AuthorUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OriginalUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TweetDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsSeen = table.Column<bool>(type: "bit", nullable: false),
                    PlatformName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tweets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tweets");
        }
    }
}
