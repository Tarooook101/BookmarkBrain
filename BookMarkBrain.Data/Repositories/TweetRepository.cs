using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Data.Repositories;

public class TweetRepository : Repository<Tweet>, ITweetRepository
{
    private readonly ILogger<TweetRepository> _logger;

    public TweetRepository(AppDbContext dbContext, ILogger<TweetRepository> logger)
        : base(dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<Tweet>> GetTweetsByCategoryIdAsync(Guid categoryId)
    {
        _logger.LogInformation("Getting tweets for category with ID {CategoryId}", categoryId);
        return await _dbContext.Tweets
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Tweet>> GetTweetsByPlatformAsync(string platformName)
    {
        _logger.LogInformation("Getting tweets for platform {PlatformName}", platformName);
        return await _dbContext.Tweets
            .Where(t => t.PlatformName == platformName)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Tweet>> SearchTweetsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        _logger.LogInformation("Searching tweets with term: {SearchTerm}", searchTerm);

        searchTerm = searchTerm.ToLower();
        return await _dbContext.Tweets
            .Where(t =>
                t.Content.ToLower().Contains(searchTerm) ||
                t.AuthorUsername.ToLower().Contains(searchTerm) ||
                t.PlatformName.ToLower().Contains(searchTerm))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}