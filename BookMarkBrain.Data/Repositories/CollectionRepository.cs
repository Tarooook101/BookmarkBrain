using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace BookMarkBrain.Data.Repositories;

public class CollectionRepository : Repository<Collection>, ICollectionRepository
{
    private readonly AppDbContext _context;

    public CollectionRepository(AppDbContext context) : base(context)
    {
        _context = context; // Assign the injected context to the private field
    }

    public async Task<IReadOnlyList<Collection>> GetCollectionsOrderedByDisplayOrderAsync()
    {
        return await _context.Set<Collection>()
            .OrderBy(c => c.DisplayOrder)
        .ToListAsync();
    }

    public async Task<Collection> GetCollectionWithTweetsAsync(Guid id)
    {
        return await _context.Set<Collection>()
            .Include(c => c.CollectionTweets)
                .ThenInclude(ct => ct.Tweet)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<Collection>> GetPublicCollectionsAsync()
    {
        return await _context.Set<Collection>()
            .Where(c => c.IsPublic)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Collection>> SearchCollectionsAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return await GetAllAsync();

        return await _context.Set<Collection>()
            .Where(c => c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm))
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }
}