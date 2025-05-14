using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.MVC.Models.Category;

namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;
public interface ICategoryApiService
{

    Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync();

    Task<CategoryViewModel> GetCategoryByIdAsync(Guid id);

    Task<CategoryViewModel> GetCategoryWithChildrenAsync(Guid id);

    Task<IEnumerable<CategoryViewModel>> GetRootCategoriesAsync();

    Task<IEnumerable<CategoryTreeDto>> GetCategoryTreeAsync();

    Task<CategoryViewModel> CreateCategoryAsync(CreateCategoryViewModel createViewModel);

    Task<CategoryViewModel> UpdateCategoryAsync(Guid id, UpdateCategoryViewModel updateViewModel);

    Task<bool> UpdateCategoryDisplayOrderAsync(Dictionary<Guid, int> categoryOrderUpdates);

    Task<bool> DeleteCategoryAsync(Guid id);

    Task<List<CategoryDropdownItem>> GetCategoryDropdownItemsAsync();
}