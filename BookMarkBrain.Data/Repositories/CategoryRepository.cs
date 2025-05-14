using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace BookMarkBrain.Data.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync()
    {
        return await _dbContext.Set<Category>()
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesWithChildrenAsync()
    {
        return await _dbContext.Set<Category>()
            .Include(c => c.ChildCategories)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Category> GetCategoryWithChildrenAsync(Guid id)
    {
        return await _dbContext.Set<Category>()
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> GetCategoryWithFullHierarchyAsync(Guid id)
    {
        // Load the category with its immediate children
        var category = await _dbContext.Set<Category>()
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return null;

        // For each child, recursively load its children
        foreach (var child in category.ChildCategories.ToList())
        {
            await LoadChildrenRecursively(child);
        }

        return category;
    }

    private async Task LoadChildrenRecursively(Category category)
    {
        // Load the immediate children of the current category
        await _dbContext.Entry(category)
            .Collection(c => c.ChildCategories)
            .LoadAsync();

        // Recursively load children for each child
        foreach (var child in category.ChildCategories.ToList())
        {
            await LoadChildrenRecursively(child);
        }
    }

    public async Task<bool> HasChildrenAsync(Guid id)
    {
        return await _dbContext.Set<Category>()
            .AnyAsync(c => c.ParentCategoryId == id);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesOrderedByDisplayOrderAsync()
    {
        return await _dbContext.Set<Category>()
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }
}