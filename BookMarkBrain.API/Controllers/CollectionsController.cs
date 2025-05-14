using BookMarkBrain.Core.DTOs.Collection;
using BookMarkBrain.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CollectionsController : BaseApiController
{
    private readonly ICollectionService _collectionService;

    public CollectionsController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _collectionService.GetAllAsync();
        return HandleResult(result);
    }

    [HttpGet("ordered")]
    public async Task<IActionResult> GetOrdered()
    {
        var result = await _collectionService.GetCollectionsOrderedByDisplayOrderAsync();
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _collectionService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}/with-tweets")]
    public async Task<IActionResult> GetWithTweets(Guid id)
    {
        var result = await _collectionService.GetCollectionWithTweetsAsync(id);
        return HandleResult(result);
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublicCollections()
    {
        var result = await _collectionService.GetPublicCollectionsAsync();
        return HandleResult(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var result = await _collectionService.SearchCollectionsAsync(term);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCollectionDto createCollectionDto)
    {
        var result = await _collectionService.CreateCollectionAsync(createCollectionDto);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCollectionDto updateCollectionDto)
    {
        var result = await _collectionService.UpdateCollectionAsync(id, updateCollectionDto);
        return HandleResult(result);
    }

    [HttpPut("update-order")]
    public async Task<IActionResult> UpdateDisplayOrder([FromBody] Dictionary<Guid, int> orderUpdates)
    {
        var result = await _collectionService.UpdateCollectionDisplayOrderAsync(orderUpdates);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _collectionService.DeleteAsync(id);
        return HandleResult(result);
    }
}