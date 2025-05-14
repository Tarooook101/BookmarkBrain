using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HashtagController : BaseApiController
{
    private readonly IHashtagService _hashtagService;
    private readonly ILogger<HashtagController> _logger;

    public HashtagController(IHashtagService hashtagService, ILogger<HashtagController> logger)
    {
        _hashtagService = hashtagService ?? throw new ArgumentNullException(nameof(hashtagService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all hashtags
    /// </summary>
    /// <returns>List of all hashtags</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllHashtags()
    {
        _logger.LogInformation("Getting all hashtags");
        var result = await _hashtagService.GetAllAsync();
        return HandleResult(result);
    }

    /// <summary>
    /// Get hashtag by id
    /// </summary>
    /// <param name="id">Hashtag id</param>
    /// <returns>Hashtag with specified id</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHashtagById(Guid id)
    {
        _logger.LogInformation("Getting hashtag by ID: {HashtagId}", id);
        var result = await _hashtagService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Get hashtag by name
    /// </summary>
    /// <param name="name">Hashtag name</param>
    /// <returns>Hashtag with specified name</returns>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHashtagByName(string name)
    {
        _logger.LogInformation("Getting hashtag by name: {HashtagName}", name);
        var result = await _hashtagService.GetHashtagByNameAsync(name);
        return HandleResult(result);
    }

    /// <summary>
    /// Get popular hashtags
    /// </summary>
    /// <returns>List of popular hashtags</returns>
    [HttpGet("popular")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPopularHashtags()
    {
        _logger.LogInformation("Getting popular hashtags");
        var result = await _hashtagService.GetPopularHashtagsAsync();
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new hashtag
    /// </summary>
    /// <param name="createHashtagDto">Create hashtag data</param>
    /// <returns>Created hashtag</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateHashtag([FromBody] CreateHashtagDto createHashtagDto)
    {
        _logger.LogInformation("Creating new hashtag with name: {HashtagName}", createHashtagDto.Name);
        var result = await _hashtagService.CreateHashtagAsync(createHashtagDto);

        if (result.Success)
        {
            return CreatedAtAction(nameof(GetHashtagById), new { id = result.Data.Id }, result);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing hashtag
    /// </summary>
    /// <param name="id">Hashtag id</param>
    /// <param name="updateHashtagDto">Update hashtag data</param>
    /// <returns>Updated hashtag</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateHashtag(Guid id, [FromBody] UpdateHashtagDto updateHashtagDto)
    {
        _logger.LogInformation("Updating hashtag with ID: {HashtagId}", id);
        var result = await _hashtagService.UpdateHashtagAsync(id, updateHashtagDto);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a hashtag
    /// </summary>
    /// <param name="id">Hashtag id</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHashtag(Guid id)
    {
        _logger.LogInformation("Deleting hashtag with ID: {HashtagId}", id);
        var result = await _hashtagService.DeleteAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Increment hashtag usage count
    /// </summary>
    /// <param name="id">Hashtag id</param>
    /// <returns>Updated usage count</returns>
    [HttpPatch("{id:guid}/increment-usage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IncrementUsageCount(Guid id)
    {
        _logger.LogInformation("Incrementing usage count for hashtag ID: {HashtagId}", id);
        var result = await _hashtagService.IncrementUsageCountAsync(id);
        return HandleResult(result);
    }
}