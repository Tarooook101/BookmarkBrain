using BookMarkBrain.Core.Entities;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace BookMarkBrain.Data.Seed;

public static class TweetCategorySeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            // Check if we already have tweet categories
            if (await context.TweetCategories.AnyAsync())
            {
                logger.LogInformation("TweetCategories already exist in the database. Skipping seeding.");
                return;
            }

            // Get existing tweets and categories to link them
            var tweets = await context.Tweets.Take(10).ToListAsync();
            var categories = await context.Categories.Take(5).ToListAsync();

            if (!tweets.Any() || !categories.Any())
            {
                logger.LogWarning("Cannot seed TweetCategories: No tweets or categories found in the database.");
                return;
            }

            var tweetCategories = new List<TweetCategory>();

            // Create some tweet-category relationships
            // Each tweet will be assigned to 1-3 random categories
            Random random = new Random();
            foreach (var tweet in tweets)
            {
                // Determine how many categories to assign (1-3)
                int categoryCount = random.Next(1, Math.Min(4, categories.Count + 1));

                // Get random categories
                var selectedCategories = categories
                    .OrderBy(c => Guid.NewGuid()) // Random ordering
                    .Take(categoryCount)
                    .ToList();

                foreach (var category in selectedCategories)
                {
                    tweetCategories.Add(new TweetCategory
                    {
                        Id = Guid.NewGuid(),
                        TweetId = tweet.Id,
                        CategoryId = category.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await context.TweetCategories.AddRangeAsync(tweetCategories);
            await context.SaveChangesAsync();

            logger.LogInformation("Successfully seeded {Count} tweet-category relationships", tweetCategories.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding TweetCategories");
            throw;
        }
    }
}