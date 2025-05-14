

using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.RepoInterfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync();

    Task<IReadOnlyList<Category>> GetCategoriesWithChildrenAsync();

    Task<Category> GetCategoryWithChildrenAsync(Guid id);

    Task<Category> GetCategoryWithFullHierarchyAsync(Guid id);
    Task<bool> HasChildrenAsync(Guid id);

    Task<IReadOnlyList<Category>> GetCategoriesOrderedByDisplayOrderAsync();
}