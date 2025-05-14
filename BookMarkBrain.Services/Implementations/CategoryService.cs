using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using Mapster;
using Microsoft.Extensions.Logging;


namespace BookMarkBrain.Services.Implementations;

public class CategoryService : BaseService<Category, CategoryDto>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ILogger<CategoryService> logger) : base(categoryRepository, logger)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetRootCategoriesAsync()
    {
        try
        {
            var rootCategories = await _categoryRepository.GetRootCategoriesAsync();
            var categoryDtos = rootCategories?.Adapt<IReadOnlyList<CategoryDto>>();
            return ServiceResult<IReadOnlyList<CategoryDto>>.SuccessResult(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting root categories");
            return ServiceResult<IReadOnlyList<CategoryDto>>.FailureResult($"Error getting root categories: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetCategoriesWithChildrenAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetCategoriesWithChildrenAsync();
            var categoryDtos = categories.Adapt<IReadOnlyList<CategoryDto>>();

            // Process each category to include child category IDs
            foreach (var category in categories)
            {
                var dto = categoryDtos.First(c => c.Id == category.Id);
                dto.ChildCategoryIds = category.ChildCategories?.Select(c => c.Id).ToList();
            }

            return ServiceResult<IReadOnlyList<CategoryDto>>.SuccessResult(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories with children");
            return ServiceResult<IReadOnlyList<CategoryDto>>.FailureResult($"Error getting categories with children: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CategoryDto>> GetCategoryWithChildrenAsync(Guid id)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryWithChildrenAsync(id);
            if (category == null)
                return ServiceResult<CategoryDto>.FailureResult($"Category with ID {id} not found");

            var categoryDto = category.Adapt<CategoryDto>();
            categoryDto.ChildCategoryIds = category.ChildCategories?.Select(c => c.Id).ToList();

            return ServiceResult<CategoryDto>.SuccessResult(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {CategoryId} with children", id);
            return ServiceResult<CategoryDto>.FailureResult($"Error getting category with children: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CategoryTreeDto>>> GetCategoryTreeAsync()
    {
        try
        {
            // Get all categories with their children
            var allCategories = await _categoryRepository.GetCategoriesWithChildrenAsync();

            // Filter to get just the root categories
            var rootCategories = allCategories.Where(c => c.ParentCategoryId == null).ToList();

            // Create the tree structure
            var categoryTree = new List<CategoryTreeDto>();
            foreach (var rootCategory in rootCategories)
            {
                var treeNode = rootCategory.Adapt<CategoryTreeDto>();
                PopulateCategoryTree(treeNode, allCategories);
                categoryTree.Add(treeNode);
            }

            return ServiceResult<IReadOnlyList<CategoryTreeDto>>.SuccessResult(categoryTree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category tree");
            return ServiceResult<IReadOnlyList<CategoryTreeDto>>.FailureResult($"Error getting category tree: {ex.Message}");
        }
    }

    private void PopulateCategoryTree(CategoryTreeDto parentNode, IEnumerable<Category> allCategories)
    {
        // Find all children of the current node
        var children = allCategories.Where(c => c.ParentCategoryId == parentNode.Id).OrderBy(c => c.DisplayOrder);

        foreach (var child in children)
        {
            var childNode = child.Adapt<CategoryTreeDto>();
            parentNode.Children.Add(childNode);

            // Recursively populate the child's children
            PopulateCategoryTree(childNode, allCategories);
        }
    }

    public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        try
        {
            // Validate parent category if specified
            if (createCategoryDto.ParentCategoryId.HasValue)
            {
                var parentExists = await _repository.GetByIdAsync(createCategoryDto.ParentCategoryId.Value);
                if (parentExists == null)
                    return ServiceResult<CategoryDto>.FailureResult($"Parent category with ID {createCategoryDto.ParentCategoryId} not found");
            }

            // Create new category entity
            var category = createCategoryDto.Adapt<Category>();

            // Add entity to repository
            var createdCategory = await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            // Map back to DTO
            var categoryDto = createdCategory.Adapt<CategoryDto>();

            return ServiceResult<CategoryDto>.SuccessResult(categoryDto, "Category created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return ServiceResult<CategoryDto>.FailureResult($"Error creating category: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            // Get existing category
            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null)
                return ServiceResult<CategoryDto>.FailureResult($"Category with ID {id} not found");

            // Validate parent category if specified
            if (updateCategoryDto.ParentCategoryId.HasValue)
            {
                // Check if parent exists
                var parentExists = await _repository.GetByIdAsync(updateCategoryDto.ParentCategoryId.Value);
                if (parentExists == null)
                    return ServiceResult<CategoryDto>.FailureResult($"Parent category with ID {updateCategoryDto.ParentCategoryId} not found");

                // Check for circular reference (category can't be its own parent or ancestor)
                if (updateCategoryDto.ParentCategoryId.Value == id ||
                    await IsCircularReference(id, updateCategoryDto.ParentCategoryId.Value))
                    return ServiceResult<CategoryDto>.FailureResult("Circular reference detected in category hierarchy");
            }

            // Update properties
            updateCategoryDto.Adapt(existingCategory);

            // Save changes
            await _repository.UpdateAsync(existingCategory);
            await _repository.SaveChangesAsync();

            // Map to DTO
            var categoryDto = existingCategory.Adapt<CategoryDto>();

            return ServiceResult<CategoryDto>.SuccessResult(categoryDto, "Category updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
            return ServiceResult<CategoryDto>.FailureResult($"Error updating category: {ex.Message}");
        }
    }

    private async Task<bool> IsCircularReference(Guid categoryId, Guid parentId)
    {
        // Get the parent category
        var parent = await _repository.GetByIdAsync(parentId);
        if (parent == null)
            return false;

        // If this parent has a parent, check if it's our category or continue traversing up
        if (parent.ParentCategoryId.HasValue)
        {
            if (parent.ParentCategoryId.Value == categoryId)
                return true;

            return await IsCircularReference(categoryId, parent.ParentCategoryId.Value);
        }

        return false;
    }
    public async Task<ServiceResult<bool>> UpdateCategoryDisplayOrderAsync(Dictionary<Guid, int> categoryOrderUpdates)
    {
        try
        {
            foreach (var update in categoryOrderUpdates)
            {
                var category = await _repository.GetByIdAsync(update.Key);
                if (category == null)
                    return ServiceResult<bool>.FailureResult($"Category with ID {update.Key} not found");

                category.DisplayOrder = update.Value;
                await _repository.UpdateAsync(category);
            }

            await _repository.SaveChangesAsync();
            return ServiceResult<bool>.SuccessResult(true, "Category display orders updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category display orders");
            return ServiceResult<bool>.FailureResult($"Error updating category display orders: {ex.Message}");
        }
    }
}