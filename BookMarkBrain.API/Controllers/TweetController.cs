using BookMarkBrain.Core.DTOs.Tweet;
using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TweetController : BaseApiController
{
    private readonly ITweetService _tweetService;
    private readonly ILogger<TweetController> _logger;

    public TweetController(ITweetService tweetService, ILogger<TweetController> logger)
    {
        _tweetService = tweetService ?? throw new ArgumentNullException(nameof(tweetService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTweets()
    {
        _logger.LogInformation("Getting all tweets");
        var result = await _tweetService.GetAllAsync();
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTweetById(Guid id)
    {
        _logger.LogInformation("Getting tweet with ID: {TweetId}", id);
        var result = await _tweetService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetTweetsByCategory(Guid categoryId)
    {
        _logger.LogInformation("Getting tweets for category with ID: {CategoryId}", categoryId);
        var result = await _tweetService.GetTweetsByCategoryIdAsync(categoryId);
        return HandleResult(result);
    }

    [HttpGet("platform/{platformName}")]
    public async Task<IActionResult> GetTweetsByPlatform(string platformName)
    {
        _logger.LogInformation("Getting tweets for platform: {PlatformName}", platformName);
        var result = await _tweetService.GetTweetsByPlatformAsync(platformName);
        return HandleResult(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTweets([FromQuery] string term)
    {
        _logger.LogInformation("Searching tweets with term: {SearchTerm}", term);
        var result = await _tweetService.SearchTweetsAsync(term);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTweet([FromBody] TweetDto tweetDto)
    {
        _logger.LogInformation("Creating new tweet");
        var result = await _tweetService.CreateAsync(tweetDto);
        return HandleResult(result);
    }

    [HttpPost("extract")]
    public async Task<IActionResult> ExtractTweetFromUrl([FromBody] UrlDto urlDto)
    {
        _logger.LogInformation("Extracting tweet from URL: {Url}", urlDto.Url);
        var result = await _tweetService.ExtractTweetFromUrlAsync(urlDto.Url);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTweet(Guid id, [FromBody] TweetDto tweetDto)
    {
        _logger.LogInformation("Updating tweet with ID: {TweetId}", id);
        var result = await _tweetService.UpdateAsync(id, tweetDto);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/toggle-seen")]
    public async Task<IActionResult> ToggleTweetSeenStatus(Guid id)
    {
        _logger.LogInformation("Toggling seen status for tweet with ID: {TweetId}", id);
        var result = await _tweetService.ToggleSeenStatusAsync(id);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTweet(Guid id)
    {
        _logger.LogInformation("Deleting tweet with ID: {TweetId}", id);
        var result = await _tweetService.DeleteAsync(id);
        return HandleResult(result);
    }
}

// Add this DTO for URL extraction
public class UrlDto
{
    public string Url { get; set; }
}
