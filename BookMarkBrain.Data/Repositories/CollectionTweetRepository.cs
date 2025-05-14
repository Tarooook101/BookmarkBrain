using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace BookMarkBrain.Data.Repositories;

public class CollectionTweetRepository : Repository<CollectionTweet>, ICollectionTweetRepository
{
    private readonly AppDbContext _context;

    public CollectionTweetRepository(AppDbContext context) : base(context)
    {
        _context = context; // Assign the injected context to the private field
    }

    public async Task<IReadOnlyList<CollectionTweet>> GetByCollectionIdAsync(Guid collectionId)
    {
        return await _context.Set<CollectionTweet>()
            .Where(ct => ct.CollectionId == collectionId)
        .ToListAsync();
    }

    public async Task<IReadOnlyList<CollectionTweet>> GetByTweetIdAsync(Guid tweetId)
    {
        return await _context.Set<CollectionTweet>()
            .Where(ct => ct.TweetId == tweetId)
        .ToListAsync();
    }

    public async Task<CollectionTweet> GetByCollectionAndTweetIdAsync(Guid collectionId, Guid tweetId)
    {
        return await _context.Set<CollectionTweet>()
            .FirstOrDefaultAsync(ct => ct.CollectionId == collectionId && ct.TweetId == tweetId);
    }

    public async Task<IReadOnlyList<CollectionTweet>> GetAllWithDetailsAsync()
    {
        return await _context.Set<CollectionTweet>()
            .Include(ct => ct.Collection)
            .Include(ct => ct.Tweet)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CollectionTweet>> GetByCollectionIdOrderedByDisplayOrderAsync(Guid collectionId)
    {
        return await _context.Set<CollectionTweet>()
            .Where(ct => ct.CollectionId == collectionId)
            .OrderBy(ct => ct.DisplayOrder)
            .Include(ct => ct.Tweet)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<CollectionTweet> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize)
    {
        var totalCount = await _context.Set<CollectionTweet>().CountAsync();

        var items = await _context.Set<CollectionTweet>()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Include(ct => ct.Collection)
            .Include(ct => ct.Tweet)
            .ToListAsync();

        return (items, totalCount);
    }
}