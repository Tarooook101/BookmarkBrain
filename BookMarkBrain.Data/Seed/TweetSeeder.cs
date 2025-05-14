using BookMarkBrain.Core.Entities;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Data.Seed;

public static class TweetSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        // Check if any tweets already exist
        if (await context.Tweets.AnyAsync())
        {
            logger.LogInformation("Tweet data already exists - skipping seeding");
            return;
        }

        logger.LogInformation("Seeding Tweet data...");

        var tweets = new List<Tweet>
        {
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Understanding the fundamentals of #Operating_System is crucial for any software developer. Here's why thread management matters...",
                AuthorUsername = "techguru",
                OriginalUrl = "https://twitter.com/techguru/status/1234567890",
                TweetDate = DateTime.UtcNow.AddDays(-5),
                ImageUrl = "https://example.com/images/os-threads.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Did you know that modern #Operating_System design relies heavily on microkernel architecture? This approach allows for better isolation...",
                AuthorUsername = "systemexpert",
                OriginalUrl = "https://twitter.com/systemexpert/status/1234567891",
                TweetDate = DateTime.UtcNow.AddDays(-3),
                ImageUrl = "https://example.com/images/microkernel.jpg",
                IsSeen = true,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "#ASPNETCore 9 is bringing some amazing performance improvements. Here's how the new middleware pipeline works...",
                AuthorUsername = "dotnetdev",
                OriginalUrl = "https://twitter.com/dotnetdev/status/1234567892",
                TweetDate = DateTime.UtcNow.AddDays(-2),
                ImageUrl = "https://example.com/images/aspnet-core.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Exploring the relationship between #NoSQL databases and traditional RDBMS. When should you choose one over the other?",
                AuthorUsername = "databasewhiz",
                OriginalUrl = "https://twitter.com/databasewhiz/status/1234567893",
                TweetDate = DateTime.UtcNow.AddDays(-1),
                ImageUrl = "https://example.com/images/database-comparison.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Top 5 #Docker best practices for production environments that will save you from common pitfalls...",
                AuthorUsername = "containerspecialist",
                OriginalUrl = "https://twitter.com/containerspecialist/status/1234567894",
                TweetDate = DateTime.UtcNow.AddHours(-12),
                ImageUrl = "https://example.com/images/docker-prod.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Why #Clean_Architecture matters in enterprise applications. A thread on maintainability and scalability...",
                AuthorUsername = "architectureguru",
                OriginalUrl = "https://twitter.com/architectureguru/status/1234567895",
                TweetDate = DateTime.UtcNow.AddDays(-4),
                ImageUrl = "https://example.com/images/clean-architecture.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "The future of #WebDevelopment is here with Web Assembly. Check out this demo of a fully client-side ML model...",
                AuthorUsername = "webdevleader",
                OriginalUrl = "https://twitter.com/webdevleader/status/1234567896",
                TweetDate = DateTime.UtcNow.AddDays(-6),
                ImageUrl = "https://example.com/images/web-assembly.jpg",
                IsSeen = true,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Interesting read on how #AI_ML is transforming database query optimization. The results are impressive!",
                AuthorUsername = "aidatascientist",
                OriginalUrl = "https://twitter.com/aidatascientist/status/1234567897",
                TweetDate = DateTime.UtcNow.AddDays(-2),
                ImageUrl = "https://example.com/images/ai-database.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Important security advisory for all #DotNet developers: Here's how to protect your apps from the latest vulnerability...",
                AuthorUsername = "securityalert",
                OriginalUrl = "https://twitter.com/securityalert/status/1234567898",
                TweetDate = DateTime.UtcNow.AddHours(-36),
                ImageUrl = "https://example.com/images/security-patch.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Tweet
            {
                Id = Guid.NewGuid(),
                Content = "Just published my new tutorial on building scalable microservices with #Kubernetes and #DotNet. Link in bio!",
                AuthorUsername = "cloudarchitect",
                OriginalUrl = "https://twitter.com/cloudarchitect/status/1234567899",
                TweetDate = DateTime.UtcNow.AddHours(-18),
                ImageUrl = "https://example.com/images/kubernetes-tutorial.jpg",
                IsSeen = false,
                PlatformName = "Twitter",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        await context.Tweets.AddRangeAsync(tweets);
        await context.SaveChangesAsync();
        logger.LogInformation("Successfully seeded {Count} tweets", tweets.Count);
    }
}