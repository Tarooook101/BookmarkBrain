using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Models.Category;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Mapster;

namespace BookMarkBrain.MVC.Services.APIServiceImplementations;

/// <summary>
/// Implementation of the Category API service
/// </summary>
public class CategoryApiService : ICategoryApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<CategoryApiService> _logger;
    private const string BaseEndpoint = "api/category";

    public CategoryApiService(IApiService apiService, ILogger<CategoryApiService> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync()
    {
        try
        {
            _logger.LogInformation("Getting all categories");
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CategoryDto>>>(BaseEndpoint);

            if (result.Success && result.Data != null)
            {
                return result.Data.Adapt<IEnumerable<CategoryViewModel>>();
            }

            _logger.LogWarning("Failed to get categories: {Message}", result.Message);
            return Enumerable.Empty<CategoryViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories");
            throw;
        }
    }

    public async Task<CategoryViewModel> GetCategoryByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting category with ID: {CategoryId}", id);
            var result = await _apiService.GetAsync<ServiceResult<CategoryDto>>($"{BaseEndpoint}/{id}");

            if (result.Success && result.Data != null)
            {
                return result.Data.Adapt<CategoryViewModel>();
            }

            _logger.LogWarning("Failed to get category with ID {CategoryId}: {Message}", id, result.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with ID {CategoryId}", id);
            throw;
        }
    }

    public async Task<CategoryViewModel> GetCategoryWithChildrenAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting category with children for ID: {CategoryId}", id);
            var result = await _apiService.GetAsync<ServiceResult<CategoryDto>>($"{BaseEndpoint}/{id}/with-children");

            if (result.Success && result.Data != null)
            {
                return result.Data.Adapt<CategoryViewModel>();
            }

            _logger.LogWarning("Failed to get category with children for ID {CategoryId}: {Message}", id, result.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with children for ID {CategoryId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<CategoryViewModel>> GetRootCategoriesAsync()
    {
        try
        {
            _logger.LogInformation("Getting root categories");
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CategoryDto>>>($"{BaseEndpoint}/root");

            if (result.Success && result.Data != null)
            {
                return result.Data.Adapt<IEnumerable<CategoryViewModel>>();
            }

            _logger.LogWarning("Failed to get root categories: {Message}", result.Message);
            return Enumerable.Empty<CategoryViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting root categories");
            throw;
        }
    }

    public async Task<IEnumerable<CategoryTreeDto>> GetCategoryTreeAsync()
    {
        try
        {
            _logger.LogInformation("Getting category tree");
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CategoryTreeDto>>>($"{BaseEndpoint}/tree");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            _logger.LogWarning("Failed to get category tree: {Message}", result.Message);
            return Enumerable.Empty<CategoryTreeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category tree");
            throw;
        }
    }

    public async Task<CategoryViewModel> CreateCategoryAsync(CreateCategoryViewModel createViewModel)
    {
        try
        {
            _logger.LogInformation("Creating new category: {CategoryName}", createViewModel.Name);
            var createDto = createViewModel.Adapt<CreateCategoryDto>();

            var result = await _apiService.PostAsync<CreateCategoryDto, ServiceResult<CategoryDto>>(
                BaseEndpoint, createDto);

            if (result.Success && result.Data != null)
            {
                return result.Data.Adapt<CategoryViewModel>();
            }

            _logger.LogWarning("Failed to create category: {Message}", result.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            throw;
        }
    }

    public async Task<CategoryViewModel> UpdateCategoryAsync(Guid id, UpdateCategoryViewModel updateViewModel)
    {
        try
        {
            _logger.LogInformation("Updating category with ID: {CategoryId}", id);
            var updateDto = updateViewModel.Adapt<UpdateCategoryDto>();

            var result = await _apiService.PutAsync<UpdateCategoryDto, ServiceResult<CategoryDto>>(
                $"{BaseEndpoint}/{id}", updateDto);

            if (result.Success && result.Data != null)
            {
                return result.Data.Adapt<CategoryViewModel>();
            }

            _logger.LogWarning("Failed to update category with ID {CategoryId}: {Message}", id, result.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
            throw;
        }
    }

    public async Task<bool> UpdateCategoryDisplayOrderAsync(Dictionary<Guid, int> categoryOrderUpdates)
    {
        try
        {
            _logger.LogInformation("Updating display order for {Count} categories", categoryOrderUpdates.Count);
            var result = await _apiService.PutAsync<Dictionary<Guid, int>, ServiceResult<bool>>(
                $"{BaseEndpoint}/display-order", categoryOrderUpdates);

            if (result.Success)
            {
                return result.Data;
            }

            _logger.LogWarning("Failed to update category display orders: {Message}", result.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category display orders");
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
            var result = await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
            throw;
        }
    }

    public async Task<List<CategoryDropdownItem>> GetCategoryDropdownItemsAsync()
    {
        try
        {
            _logger.LogInformation("Getting categories for dropdown");
            var treeResult = await GetCategoryTreeAsync();
            var dropdownItems = new List<CategoryDropdownItem>();

            // Generate flat list with hierarchy information
            ProcessCategoryTreeForDropdown(treeResult.ToList(), dropdownItems);

            return dropdownItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category dropdown items");
            throw;
        }
    }

    private void ProcessCategoryTreeForDropdown(
        IEnumerable<CategoryTreeDto> categories,
        List<CategoryDropdownItem> result,
        int level = 0,
        string prefix = "")
    {
        foreach (var category in categories)
        {
            var indentation = level > 0 ? $"{prefix}└─ " : "";

            result.Add(new CategoryDropdownItem
            {
                Id = category.Id,
                Name = $"{indentation}{category.Name}",
                Level = level
            });

            if (category.Children != null && category.Children.Any())
            {
                var newPrefix = level > 0 ? $"{prefix}  " : "  ";
                ProcessCategoryTreeForDropdown(category.Children, result, level + 1, newPrefix);
            }
        }
    }
}