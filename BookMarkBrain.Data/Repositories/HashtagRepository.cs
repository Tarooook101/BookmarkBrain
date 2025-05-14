using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMarkBrain.Data.Repositories;
public class HashtagRepository : Repository<Hashtag>, IHashtagRepository
{
    public HashtagRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Hashtag> GetByNameAsync(string name)
    {
        return await _dbContext.Hashtags
            .FirstOrDefaultAsync(h => h.Name.ToLower() == name.ToLower());
    }

    public async Task<IReadOnlyList<Hashtag>> GetPopularHashtagsAsync()
    {
        return await _dbContext.Hashtags
            .Where(h => h.IsPopular)
            .OrderByDescending(h => h.UsageCount)
            .ToListAsync();
    }

    public async Task<int> IncrementUsageCountAsync(Guid id)
    {
        var hashtag = await _dbContext.Hashtags.FindAsync(id);
        if (hashtag == null)
            return 0;

        hashtag.UsageCount++;

        // Check if hashtag should now be marked as popular (based on usage count threshold)
        if (hashtag.UsageCount >= 10 && !hashtag.IsPopular)
        {
            hashtag.IsPopular = true;
        }

        await _dbContext.SaveChangesAsync();
        return hashtag.UsageCount;
    }
}