using BookMarkBrain.Core.Entities;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Data.Seed;

public static class TweetHashtagSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        // Check if any tweet hashtags already exist
        if (await context.TweetHashtags.AnyAsync())
        {
            logger.LogInformation("TweetHashtag data already exists - skipping seeding");
            return;
        }

        logger.LogInformation("Seeding TweetHashtag data...");

        // Get existing tweets and hashtags to create relationships
        var tweets = await context.Tweets.ToListAsync();
        var hashtags = await context.Hashtags.ToListAsync();

        if (!tweets.Any() || !hashtags.Any())
        {
            logger.LogWarning("Cannot seed TweetHashtags: No tweets or hashtags found in the database");
            return;
        }

        var tweetHashtags = new List<TweetHashtag>();

        // Find hashtag by name helper function
        Hashtag FindHashtagByName(string name) =>
            hashtags.FirstOrDefault(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        // Process each tweet and extract hashtags
        foreach (var tweet in tweets)
        {
            await ProcessTweetHashtags(context, tweet, hashtags, tweetHashtags, FindHashtagByName);
        }

        // Remove any duplicate tweet-hashtag combinations
        tweetHashtags = tweetHashtags
            .GroupBy(th => new { th.TweetId, th.HashtagId })
            .Select(g => g.First())
            .ToList();

        // Add the tweet hashtags to the database
        if (tweetHashtags.Any())
        {
            await context.TweetHashtags.AddRangeAsync(tweetHashtags);
            await context.SaveChangesAsync();

            // Update hashtag usage counts
            var hashtagUsageCounts = tweetHashtags
                .GroupBy(th => th.HashtagId)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var hashtag in hashtags)
            {
                if (hashtagUsageCounts.TryGetValue(hashtag.Id, out int count))
                {
                    hashtag.UsageCount += count;
                }
            }
            await context.SaveChangesAsync();

            logger.LogInformation("Successfully seeded {Count} tweet hashtags", tweetHashtags.Count);
        }
        else
        {
            logger.LogWarning("No tweet hashtags were created during seeding");
        }
    }

    private static async Task ProcessTweetHashtags(
        AppDbContext context,
        Tweet tweet,
        List<Hashtag> hashtags,
        List<TweetHashtag> tweetHashtags,
        Func<string, Hashtag> findHashtagByName)
    {
        var content = tweet.Content.ToLower();

        // Programming related tweets
        if (content.Contains("programming") || content.Contains("#programming"))
        {
            AddHashtagToTweet(tweet.Id, findHashtagByName("programming"), tweetHashtags);
        }

        // .NET related tweets
        if (content.Contains("dotnet") || content.Contains("#dotnet") ||
            content.Contains("aspnetcore") || content.Contains("#aspnetcore") ||
            content.Contains("csharp") || content.Contains("#csharp"))
        {
            AddHashtagToTweet(tweet.Id, findHashtagByName("dotnet"), tweetHashtags);

            if (content.Contains("csharp") || content.Contains("#csharp"))
            {
                AddHashtagToTweet(tweet.Id, findHashtagByName("csharp"), tweetHashtags);
            }

            if (content.Contains("aspnetcore") || content.Contains("#aspnetcore"))
            {
                AddHashtagToTweet(tweet.Id, findHashtagByName("aspnetcore"), tweetHashtags);
            }
        }

        // JavaScript related tweets
        if (content.Contains("javascript") || content.Contains("#javascript"))
        {
            AddHashtagToTweet(tweet.Id, findHashtagByName("javascript"), tweetHashtags);
        }

        // Database related tweets
        if (content.Contains("database") || content.Contains("#database") ||
            content.Contains("sql") || content.Contains("nosql"))
        {
            AddHashtagToTweet(tweet.Id, findHashtagByName("database"), tweetHashtags);
        }

        // AI/ML related tweets
        if (content.Contains("ai") || content.Contains("#ai") ||
            content.Contains("machine learning") || content.Contains("ai_ml"))
        {
            AddHashtagToTweet(tweet.Id, findHashtagByName("ai"), tweetHashtags);
        }

        // BookMarkBrain project mentions
        if (content.Contains("bookmarkbrain") || content.Contains("#bookmarkbrain"))
        {
            AddHashtagToTweet(tweet.Id, findHashtagByName("bookmarkbrain"), tweetHashtags);
        }

        // Special case hashtags that might not exist yet
        await ProcessSpecialHashtags(context, tweet, content, hashtags, tweetHashtags);
    }

    private static async Task ProcessSpecialHashtags(
        AppDbContext context,
        Tweet tweet,
        string content,
        List<Hashtag> hashtags,
        List<TweetHashtag> tweetHashtags)
    {
        // Add hashtags for special tags in the tweet content
        var specialHashtags = new Dictionary<string, string>
        {
            { "#operating_system", "operating_system" },
            { "#clean_architecture", "clean_architecture" },
            { "#webdevelopment", "webdevelopment" },
            { "#docker", "docker" },
            { "#kubernetes", "kubernetes" },
            { "#k8s", "kubernetes" }
        };

        foreach (var specialHashtag in specialHashtags)
        {
            if (content.Contains(specialHashtag.Key) || content.Contains(specialHashtag.Value))
            {
                var hashtag = hashtags.FirstOrDefault(h =>
                    h.Name.Equals(specialHashtag.Value, StringComparison.OrdinalIgnoreCase));

                if (hashtag == null)
                {
                    // Create the hashtag if it doesn't exist
                    hashtag = new Hashtag
                    {
                        Id = Guid.NewGuid(),
                        Name = specialHashtag.Value,
                        Description = $"Content related to {specialHashtag.Value.Replace('_', ' ')}",
                        IsPopular = false,
                        UsageCount = 1,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.Hashtags.Add(hashtag);
                    await context.SaveChangesAsync();
                    hashtags.Add(hashtag);
                }

                AddHashtagToTweet(tweet.Id, hashtag, tweetHashtags);
            }
        }
    }

    private static void AddHashtagToTweet(Guid tweetId, Hashtag hashtag, List<TweetHashtag> tweetHashtags)
    {
        if (hashtag != null)
        {
            tweetHashtags.Add(new TweetHashtag
            {
                Id = Guid.NewGuid(),
                TweetId = tweetId,
                HashtagId = hashtag.Id,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            });
        }
    }
}
