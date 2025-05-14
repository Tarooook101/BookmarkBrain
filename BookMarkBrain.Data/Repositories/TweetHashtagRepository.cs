using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace BookMarkBrain.Data.Repositories;


/// <summary>
/// Repository implementation for TweetHashtag entity operations
/// </summary>
public class TweetHashtagRepository : Repository<TweetHashtag>, ITweetHashtagRepository
{
    private readonly AppDbContext _dbContext;

    public TweetHashtagRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TweetHashtag>> GetByTweetIdAsync(Guid tweetId)
    {
        return await _dbContext.Set<TweetHashtag>()
            .Where(th => th.TweetId == tweetId)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TweetHashtag>> GetByHashtagIdAsync(Guid hashtagId)
    {
        return await _dbContext.Set<TweetHashtag>()
            .Where(th => th.HashtagId == hashtagId)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TweetHashtag> GetByTweetAndHashtagIdAsync(Guid tweetId, Guid hashtagId)
    {
        return await _dbContext.Set<TweetHashtag>()
            .FirstOrDefaultAsync(th => th.TweetId == tweetId && th.HashtagId == hashtagId);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TweetHashtag>> GetAllWithDetailsAsync()
    {
        return await _dbContext.Set<TweetHashtag>()
            .Include(th => th.Tweet)
            .Include(th => th.Hashtag)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TweetHashtag>> GetByTweetIdWithDetailsAsync(Guid tweetId)
    {
        return await _dbContext.Set<TweetHashtag>()
            .Include(th => th.Hashtag)
            .Where(th => th.TweetId == tweetId)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TweetHashtag>> GetByHashtagIdWithDetailsAsync(Guid hashtagId)
    {
        return await _dbContext.Set<TweetHashtag>()
            .Include(th => th.Tweet)
            .Where(th => th.HashtagId == hashtagId)
            .ToListAsync();
    }
}