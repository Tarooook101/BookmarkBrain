using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController : BaseApiController
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        _logger.LogInformation("Getting all categories");
        var result = await _categoryService.GetAllAsync();
        return HandleResult(result);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        _logger.LogInformation("Getting category tree");
        var result = await _categoryService.GetCategoryTreeAsync();
        return HandleResult(result);
    }

    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories()
    {
        _logger.LogInformation("Getting root categories");
        var result = await _categoryService.GetRootCategoriesAsync();
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        _logger.LogInformation("Getting category with ID: {CategoryId}", id);
        var result = await _categoryService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("{id}/with-children")]
    public async Task<IActionResult> GetCategoryWithChildren(Guid id)
    {
        _logger.LogInformation("Getting category with children for ID: {CategoryId}", id);
        var result = await _categoryService.GetCategoryWithChildrenAsync(id);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        _logger.LogInformation("Creating new category");
        var result = await _categoryService.CreateCategoryAsync(createCategoryDto);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", id);
        var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
        return HandleResult(result);
    }

    [HttpPut("display-order")]
    public async Task<IActionResult> UpdateCategoryDisplayOrder([FromBody] Dictionary<Guid, int> categoryOrderUpdates)
    {
        _logger.LogInformation("Updating category display orders");
        var result = await _categoryService.UpdateCategoryDisplayOrderAsync(categoryOrderUpdates);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
        var result = await _categoryService.DeleteAsync(id);
        return HandleResult(result);
    }
}