using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.ServiceInterfaces;

public interface ICategoryService : IBaseService<Category, CategoryDto>
{
    Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetRootCategoriesAsync();

    Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetCategoriesWithChildrenAsync();

    Task<ServiceResult<CategoryDto>> GetCategoryWithChildrenAsync(Guid id);

    Task<ServiceResult<IReadOnlyList<CategoryTreeDto>>> GetCategoryTreeAsync();

    Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

    Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto);

    Task<ServiceResult<bool>> UpdateCategoryDisplayOrderAsync(Dictionary<Guid, int> categoryOrderUpdates);
}