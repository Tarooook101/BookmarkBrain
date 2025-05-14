using BookMarkBrain.Core.Entities;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Data.Seed;

public static class CollectionSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        // Check if any collections already exist
        if (await context.Collections.AnyAsync())
        {
            logger.LogInformation("Collection data already exists - skipping seeding");

            // Check if any CollectionTweets exist
            if (!await context.CollectionTweets.AnyAsync())
            {
                logger.LogInformation("No CollectionTweet data found, but Collections exist. You may want to seed CollectionTweets separately.");
            }

            return;
        }

        logger.LogInformation("Seeding Collection data...");

        // Create collections
        var collections = await CreateCollections(context);

        // Create collection-tweet relationships
        await CreateCollectionTweets(context, collections, logger);
    }

    private static async Task<List<Collection>> CreateCollections(AppDbContext context)
    {
        var collections = new List<Collection>
        {
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = "Programming Basics",
                Description = "Fundamental concepts and practices in software development",
                IconUrl = "https://example.com/icons/programming.png",
                IsPublic = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = "Advanced Database Concepts",
                Description = "Deep dives into database design, optimization, and management",
                IconUrl = "https://example.com/icons/database.png",
                IsPublic = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = ".NET Development",
                Description = "Best practices and tips for .NET and ASP.NET Core development",
                IconUrl = "https://example.com/icons/dotnet.png",
                IsPublic = true,
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = "Cloud Architecture",
                Description = "Patterns and practices for building scalable cloud solutions",
                IconUrl = "https://example.com/icons/cloud.png",
                IsPublic = true,
                DisplayOrder = 4,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = "DevOps Essentials",
                Description = "CI/CD, container orchestration, and infrastructure as code",
                IconUrl = "https://example.com/icons/devops.png",
                IsPublic = true,
                DisplayOrder = 5,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = "Software Architecture",
                Description = "Design patterns, clean architecture, and system design principles",
                IconUrl = "https://example.com/icons/architecture.png",
                IsPublic = false,
                DisplayOrder = 6,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = "AI and Machine Learning",
                Description = "Resources on artificial intelligence, machine learning, and data science",
                IconUrl = "https://example.com/icons/ai.png",
                IsPublic = true,
                DisplayOrder = 7,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Name = "Security Best Practices",
                Description = "Application security, OWASP top 10, and secure coding guidelines",
                IconUrl = "https://example.com/icons/security.png",
                IsPublic = false,
                DisplayOrder = 8,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        await context.Collections.AddRangeAsync(collections);
        await context.SaveChangesAsync();

        return collections;
    }

    private static async Task CreateCollectionTweets(AppDbContext context, List<Collection> collections, ILogger logger)
    {
        // Get tweet IDs (using the tweets that should already be seeded)
        var tweets = await context.Tweets.ToListAsync();

        if (!tweets.Any())
        {
            logger.LogWarning("No tweets found in database. Skipping CollectionTweet seeding.");
            return;
        }

        logger.LogInformation("Seeding CollectionTweet data...");

        var collectionTweets = new List<CollectionTweet>();

        // Helper to find tweet by index
        Tweet GetTweetByIndex(int index) => index < tweets.Count ? tweets[index] : null;

        // Programming Basics Collection - assign relevant tweets
        AddCollectionTweet(collectionTweets, collections[0].Id, GetTweetByIndex(2)?.Id, 1); // ASP.NET Core tweet
        AddCollectionTweet(collectionTweets, collections[0].Id, GetTweetByIndex(6)?.Id, 2); // Web Development tweet
        AddCollectionTweet(collectionTweets, collections[0].Id, GetTweetByIndex(0)?.Id, 3); // OS fundamentals
        AddCollectionTweet(collectionTweets, collections[0].Id, GetTweetByIndex(1)?.Id, 4); // OS microkernel

        // Advanced Database Collection - assign relevant tweets
        AddCollectionTweet(collectionTweets, collections[1].Id, GetTweetByIndex(3)?.Id, 1); // NoSQL tweet
        AddCollectionTweet(collectionTweets, collections[1].Id, GetTweetByIndex(7)?.Id, 2); // AI/ML database tweet

        // .NET Development Collection
        AddCollectionTweet(collectionTweets, collections[2].Id, GetTweetByIndex(2)?.Id, 1); // ASP.NET Core
        AddCollectionTweet(collectionTweets, collections[2].Id, GetTweetByIndex(8)?.Id, 2); // .NET security
        AddCollectionTweet(collectionTweets, collections[2].Id, GetTweetByIndex(9)?.Id, 3); // Microservices

        // Cloud Architecture Collection
        AddCollectionTweet(collectionTweets, collections[3].Id, GetTweetByIndex(9)?.Id, 1); // Kubernetes microservices
        AddCollectionTweet(collectionTweets, collections[3].Id, GetTweetByIndex(5)?.Id, 2); // Clean Architecture

        // DevOps Essentials Collection
        AddCollectionTweet(collectionTweets, collections[4].Id, GetTweetByIndex(4)?.Id, 1); // Docker best practices
        AddCollectionTweet(collectionTweets, collections[4].Id, GetTweetByIndex(9)?.Id, 2); // Kubernetes microservices

        // Software Architecture Collection
        AddCollectionTweet(collectionTweets, collections[5].Id, GetTweetByIndex(5)?.Id, 1); // Clean Architecture
        AddCollectionTweet(collectionTweets, collections[5].Id, GetTweetByIndex(2)?.Id, 2); // ASP.NET Core middleware

        // AI and Machine Learning Collection
        AddCollectionTweet(collectionTweets, collections[6].Id, GetTweetByIndex(7)?.Id, 1); // AI database optimization

        // Security Best Practices Collection
        AddCollectionTweet(collectionTweets, collections[7].Id, GetTweetByIndex(8)?.Id, 1); // .NET security advisory

        // Save the collection-tweet relationships
        if (collectionTweets.Any())
        {
            await context.CollectionTweets.AddRangeAsync(collectionTweets);
            await context.SaveChangesAsync();
            logger.LogInformation("Successfully seeded {Count} collection-tweet relationships", collectionTweets.Count);
        }
        else
        {
            logger.LogWarning("No collection-tweet relationships were created during seeding");
        }
    }

    private static void AddCollectionTweet(
        List<CollectionTweet> collectionTweets,
        Guid collectionId,
        Guid? tweetId,
        int displayOrder)
    {
        if (tweetId.HasValue)
        {
            collectionTweets.Add(new CollectionTweet
            {
                Id = Guid.NewGuid(),
                CollectionId = collectionId,
                TweetId = tweetId.Value,
                DisplayOrder = displayOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            });
        }
    }
}