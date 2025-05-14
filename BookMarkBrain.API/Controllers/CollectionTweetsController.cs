using BookMarkBrain.Core.DTOs.CollectionTweet;
using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CollectionTweetsController : BaseApiController
{
    private readonly ICollectionTweetService _collectionTweetService;

    public CollectionTweetsController(ICollectionTweetService collectionTweetService)
    {
        _collectionTweetService = collectionTweetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _collectionTweetService.GetAllAsync();
        return HandleResult(result);
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetAllWithDetails()
    {
        var result = await _collectionTweetService.GetAllWithDetailsAsync();
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _collectionTweetService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("collection/{collectionId:guid}")]
    public async Task<IActionResult> GetByCollectionId(Guid collectionId)
    {
        var result = await _collectionTweetService.GetByCollectionIdAsync(collectionId);
        return HandleResult(result);
    }

    [HttpGet("collection/{collectionId:guid}/ordered")]
    public async Task<IActionResult> GetByCollectionIdOrdered(Guid collectionId)
    {
        var result = await _collectionTweetService.GetByCollectionIdOrderedByDisplayOrderAsync(collectionId);
        return HandleResult(result);
    }

    [HttpGet("tweet/{tweetId:guid}")]
    public async Task<IActionResult> GetByTweetId(Guid tweetId)
    {
        var result = await _collectionTweetService.GetByTweetIdAsync(tweetId);
        return HandleResult(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
    {
        var result = await _collectionTweetService.GetPagedAsync(pageIndex, pageSize);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCollectionTweetDto createDto)
    {
        var result = await _collectionTweetService.CreateCollectionTweetAsync(createDto);
        return HandleResult(result);
    }

    [HttpPost("bulk-assign")]
    public async Task<IActionResult> AssignTweetsToCollection([FromBody] BulkAssignDto bulkAssignDto)
    {
        var result = await _collectionTweetService.AssignTweetsToCollectionAsync(bulkAssignDto.CollectionId, bulkAssignDto.ItemIds);
        return HandleResult(result);
    }

    [HttpDelete("{collectionId:guid}/{tweetId:guid}")]
    public async Task<IActionResult> Delete(Guid collectionId, Guid tweetId)
    {
        var result = await _collectionTweetService.RemoveTweetFromCollectionAsync(collectionId, tweetId);
        return HandleResult(result);
    }

    [HttpPut("update-order")]
    public async Task<IActionResult> UpdateDisplayOrder([FromBody] UpdateTweetOrderDto updateOrderDto)
    {
        var result = await _collectionTweetService.UpdateTweetDisplayOrderInCollectionAsync(
            updateOrderDto.CollectionId,
            updateOrderDto.OrderUpdates);
        return HandleResult(result);
    }
}

// DTOs for bulk operations
public class BulkAssignDto
{
    public Guid CollectionId { get; set; }
    public List<Guid> ItemIds { get; set; } = new List<Guid>();
}

public class UpdateTweetOrderDto
{
    public Guid CollectionId { get; set; }
    public Dictionary<Guid, int> OrderUpdates { get; set; } = new Dictionary<Guid, int>();
}