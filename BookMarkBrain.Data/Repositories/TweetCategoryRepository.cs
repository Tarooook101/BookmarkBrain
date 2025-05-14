using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMarkBrain.Data.Repositories;
public class TweetCategoryRepository : Repository<TweetCategory>, ITweetCategoryRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<TweetCategoryRepository> _logger;

    public TweetCategoryRepository(AppDbContext context, ILogger<TweetCategoryRepository> logger) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<TweetCategory>> GetByTweetIdAsync(Guid tweetId)
    {
        try
        {
            return await _context.TweetCategories
                .Include(tc => tc.Category)
                .Where(tc => tc.TweetId == tweetId && !tc.IsDeleted)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet categories for tweet ID {TweetId}", tweetId);
            throw;
        }
    }

    public async Task<IReadOnlyList<TweetCategory>> GetByCategoryIdAsync(Guid categoryId)
    {
        try
        {
            return await _context.TweetCategories
                .Include(tc => tc.Tweet)
                .Where(tc => tc.CategoryId == categoryId && !tc.IsDeleted)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet categories for category ID {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<TweetCategory> GetByTweetAndCategoryIdAsync(Guid tweetId, Guid categoryId)
    {
        try
        {
            return await _context.TweetCategories
                .FirstOrDefaultAsync(tc =>
                    tc.TweetId == tweetId &&
                    tc.CategoryId == categoryId &&
                    !tc.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet category for tweet ID {TweetId} and category ID {CategoryId}",
                tweetId, categoryId);
            throw;
        }
    }

    public async Task<IReadOnlyList<TweetCategory>> GetAllWithDetailsAsync()
    {
        try
        {
            return await _context.TweetCategories
                .Include(tc => tc.Tweet)
                .Include(tc => tc.Category)
                .Where(tc => !tc.IsDeleted)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tweet categories with details");
            throw;
        }
    }

    public async Task<(IReadOnlyList<TweetCategory> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize)
    {
        try
        {
            var totalCount = await _context.TweetCategories
                .Where(tc => !tc.IsDeleted)
                .CountAsync();

            var items = await _context.TweetCategories
                .Include(tc => tc.Tweet)
                .Include(tc => tc.Category)
                .Where(tc => !tc.IsDeleted)
                .OrderByDescending(tc => tc.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged tweet categories for page {PageIndex} with size {PageSize}",
                pageIndex, pageSize);
            throw;
        }
    }
}