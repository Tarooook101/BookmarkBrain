using BookMarkBrain.Core.DTOs.TweetCategory;
using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TweetCategoryController : BaseApiController
{
    private readonly ITweetCategoryService _tweetCategoryService;
    private readonly ILogger<TweetCategoryController> _logger;

    public TweetCategoryController(ITweetCategoryService tweetCategoryService, ILogger<TweetCategoryController> logger)
    {
        _tweetCategoryService = tweetCategoryService ?? throw new ArgumentNullException(nameof(tweetCategoryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTweetCategories()
    {
        _logger.LogInformation("Getting all tweet categories");
        var result = await _tweetCategoryService.GetAllAsync();
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTweetCategoryById(Guid id)
    {
        _logger.LogInformation("Getting tweet category with ID: {TweetCategoryId}", id);
        var result = await _tweetCategoryService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("tweet/{tweetId:guid}")]
    public async Task<IActionResult> GetByTweetId(Guid tweetId)
    {
        _logger.LogInformation("Getting tweet categories for tweet with ID: {TweetId}", tweetId);
        var result = await _tweetCategoryService.GetByTweetIdAsync(tweetId);
        return HandleResult(result);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategoryId(Guid categoryId)
    {
        _logger.LogInformation("Getting tweet categories for category with ID: {CategoryId}", categoryId);
        var result = await _tweetCategoryService.GetByCategoryIdAsync(categoryId);
        return HandleResult(result);
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetAllWithDetails()
    {
        _logger.LogInformation("Getting all tweet categories with details");
        var result = await _tweetCategoryService.GetAllWithDetailsAsync();
        return HandleResult(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting paged tweet categories. Page {PageIndex}, Size {PageSize}", pageIndex, pageSize);
        var result = await _tweetCategoryService.GetPagedAsync(pageIndex, pageSize);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTweetCategory([FromBody] CreateTweetCategoryDto createDto)
    {
        _logger.LogInformation("Creating new tweet category");
        var result = await _tweetCategoryService.CreateTweetCategoryAsync(createDto);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTweetCategory(Guid id, [FromBody] UpdateTweetCategoryDto updateDto)
    {
        _logger.LogInformation("Updating tweet category with ID: {TweetCategoryId}", id);

        // First get the existing entity
        var existingEntity = await _tweetCategoryService.GetByIdAsync(id);
        if (!existingEntity.Success || existingEntity.Data == null)
        {
            return NotFound(ServiceResult<TweetCategoryDto>.FailureResult($"Tweet category with ID {id} not found"));
        }

        // Create a TweetCategoryDto from the existing entity and update with new values
        var tweetCategoryDto = existingEntity.Data;
        tweetCategoryDto.TweetId = updateDto.TweetId;
        tweetCategoryDto.CategoryId = updateDto.CategoryId;

        var result = await _tweetCategoryService.UpdateAsync(id, tweetCategoryDto);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTweetCategory(Guid id)
    {
        _logger.LogInformation("Deleting tweet category with ID: {TweetCategoryId}", id);
        var result = await _tweetCategoryService.DeleteAsync(id);
        return HandleResult(result);
    }

    [HttpPost("tweet/{tweetId:guid}/assign-categories")]
    public async Task<IActionResult> AssignCategoriesToTweet(Guid tweetId, [FromBody] List<Guid> categoryIds)
    {
        _logger.LogInformation("Assigning {CategoryCount} categories to tweet with ID: {TweetId}",
            categoryIds.Count, tweetId);
        var result = await _tweetCategoryService.AssignCategoriesToTweetAsync(tweetId, categoryIds);
        return HandleResult(result);
    }

    [HttpDelete("tweet/{tweetId:guid}/category/{categoryId:guid}")]
    public async Task<IActionResult> RemoveCategoryFromTweet(Guid tweetId, Guid categoryId)
    {
        _logger.LogInformation("Removing category {CategoryId} from tweet {TweetId}", categoryId, tweetId);
        var result = await _tweetCategoryService.RemoveCategoryFromTweetAsync(tweetId, categoryId);
        return HandleResult(result);
    }
}