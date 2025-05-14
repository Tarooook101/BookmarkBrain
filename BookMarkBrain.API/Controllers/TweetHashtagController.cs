using BookMarkBrain.Core.DTOs.TweetHashtag;
using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TweetHashtagController : BaseApiController
{
    private readonly ITweetHashtagService _tweetHashtagService;
    private readonly ILogger<TweetHashtagController> _logger;

    public TweetHashtagController(ITweetHashtagService tweetHashtagService, ILogger<TweetHashtagController> logger)
    {
        _tweetHashtagService = tweetHashtagService ?? throw new ArgumentNullException(nameof(tweetHashtagService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllTweetHashtags()
    {
        _logger.LogInformation("Getting all TweetHashtags");
        var result = await _tweetHashtagService.GetAllAsync();
        return HandleResult(result);
    }

    [HttpGet("details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllTweetHashtagsWithDetails()
    {
        _logger.LogInformation("Getting all TweetHashtags with details");
        var result = await _tweetHashtagService.GetAllWithDetailsAsync();
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTweetHashtagById(Guid id)
    {
        _logger.LogInformation("Getting TweetHashtag by ID: {Id}", id);
        var result = await _tweetHashtagService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("by-tweet/{tweetId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTweetHashtagsByTweetId(Guid tweetId)
    {
        _logger.LogInformation("Getting TweetHashtags by Tweet ID: {TweetId}", tweetId);
        var result = await _tweetHashtagService.GetByTweetIdAsync(tweetId);
        return HandleResult(result);
    }

    [HttpGet("by-tweet/{tweetId:guid}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTweetHashtagsByTweetIdWithDetails(Guid tweetId)
    {
        _logger.LogInformation("Getting TweetHashtags with details by Tweet ID: {TweetId}", tweetId);
        var result = await _tweetHashtagService.GetByTweetIdWithDetailsAsync(tweetId);
        return HandleResult(result);
    }

    [HttpGet("by-hashtag/{hashtagId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTweetHashtagsByHashtagId(Guid hashtagId)
    {
        _logger.LogInformation("Getting TweetHashtags by Hashtag ID: {HashtagId}", hashtagId);
        var result = await _tweetHashtagService.GetByHashtagIdAsync(hashtagId);
        return HandleResult(result);
    }

    [HttpGet("by-hashtag/{hashtagId:guid}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTweetHashtagsByHashtagIdWithDetails(Guid hashtagId)
    {
        _logger.LogInformation("Getting TweetHashtags with details by Hashtag ID: {HashtagId}", hashtagId);
        var result = await _tweetHashtagService.GetByHashtagIdWithDetailsAsync(hashtagId);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTweetHashtag([FromBody] CreateTweetHashtagDto createDto)
    {
        _logger.LogInformation("Creating TweetHashtag relationship between Tweet ID: {TweetId} and Hashtag ID: {HashtagId}",
            createDto.TweetId, createDto.HashtagId);

        var result = await _tweetHashtagService.CreateTweetHashtagAsync(createDto);

        if (result.Success)
        {
            return CreatedAtAction(nameof(GetTweetHashtagById), new { id = result.Data.Id }, result);
        }

        return HandleResult(result);
    }

    [HttpDelete("tweet/{tweetId:guid}/hashtag/{hashtagId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveTweetHashtag(Guid tweetId, Guid hashtagId)
    {
        _logger.LogInformation("Removing TweetHashtag relationship between Tweet ID: {TweetId} and Hashtag ID: {HashtagId}",
            tweetId, hashtagId);

        var result = await _tweetHashtagService.RemoveTweetHashtagAsync(tweetId, hashtagId);

        if (result.Success)
        {
            return NoContent();
        }

        return HandleResult(result);
    }
}
