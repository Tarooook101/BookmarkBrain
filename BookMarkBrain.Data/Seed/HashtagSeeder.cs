using BookMarkBrain.Core.Entities;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Data.Seed;

public static class HashtagSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        // Check if hashtags already exist
        if (await context.Hashtags.AnyAsync())
        {
            logger.LogInformation("Hashtag data already exists - skipping seeding");
            return;
        }

        logger.LogInformation("Seeding hashtag data");

        var hashtags = new List<Hashtag>
        {
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "programming",
                Description = "Content related to computer programming and software development",
                IsPopular = true,
                UsageCount = 25,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "dotnet",
                Description = "Content related to .NET platform and ecosystem",
                IsPopular = true,
                UsageCount = 18,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "csharp",
                Description = "Content related to C# programming language",
                IsPopular = true,
                UsageCount = 22,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "aspnetcore",
                Description = "Content related to ASP.NET Core web framework",
                IsPopular = true,
                UsageCount = 15,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "javascript",
                Description = "Content related to JavaScript programming language",
                IsPopular = false,
                UsageCount = 8,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "database",
                Description = "Content related to databases and data storage",
                IsPopular = false,
                UsageCount = 5,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "ai",
                Description = "Content related to artificial intelligence and machine learning",
                IsPopular = true,
                UsageCount = 12,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Hashtag
            {
                Id = Guid.NewGuid(),
                Name = "bookmarkbrain",
                Description = "Official hashtag for BookMarkBrain project discussions",
                IsPopular = false,
                UsageCount = 3,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        await context.Hashtags.AddRangeAsync(hashtags);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} hashtags", hashtags.Count);
    }
}