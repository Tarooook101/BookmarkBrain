using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;

[Route("test")]
public class CategoryTestController : Controller
{
    private readonly ICategoryApiService _categoryApiService;
    private readonly IApiService _apiService;
    private readonly ILogger<CategoryTestController> _logger;

    public CategoryTestController(
        ICategoryApiService categoryApiService,
        IApiService apiService,
        ILogger<CategoryTestController> logger)
    {
        _categoryApiService = categoryApiService ?? throw new ArgumentNullException(nameof(categoryApiService));
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: /test/categories
    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            _logger.LogInformation("Test endpoint: Getting all categories as JSON");
            var categories = await _categoryApiService.GetAllCategoriesAsync();
            return Json(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching categories in test endpoint");
            return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    // GET: /test/categories/tree
    [HttpGet("categories/tree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        try
        {
            _logger.LogInformation("Test endpoint: Getting category tree as JSON");
            var categoryTree = await _categoryApiService.GetCategoryTreeAsync();
            return Json(categoryTree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category tree in test endpoint");
            return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    // GET: /test/categories/{id}
    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        try
        {
            _logger.LogInformation("Test endpoint: Getting category {CategoryId} as JSON", id);
            var category = await _categoryApiService.GetCategoryByIdAsync(id);
            return Json(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category in test endpoint");
            return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    // GET: /test/api/categories
    [HttpGet("api/categories")]
    public async Task<IActionResult> GetRawApiCategories()
    {
        try
        {
            _logger.LogInformation("Test endpoint: Getting raw API response for categories");
            var rawResult = await _apiService.GetAsync<object>("api/Category");
            return Json(rawResult);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching raw categories from API");
            return Json(new
            {
                error = ex.Message,
                statusCode = ex.StatusCode?.ToString() ?? "Unknown",
                innerException = ex.InnerException?.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "General error fetching raw categories from API");
            return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    // GET: /test/connection
    [HttpGet("connection")]
    public IActionResult TestConnection()
    {
        return Json(new
        {
            status = "success",
            message = "Connection to MVC controller successful",
            timestamp = DateTime.UtcNow
        });
    }
}