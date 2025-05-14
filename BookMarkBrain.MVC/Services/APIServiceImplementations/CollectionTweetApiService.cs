using BookMarkBrain.Core.DTOs.CollectionTweet;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Models.CollectionTweet;
using BookMarkBrain.MVC.Models.Tweet;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceImplementations;

public class CollectionTweetApiService : ICollectionTweetApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<CollectionTweetApiService> _logger;

    public CollectionTweetApiService(IApiService apiService, ILogger<CollectionTweetApiService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<List<CollectionTweetViewModel>> GetAllCollectionTweetsAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CollectionTweetDto>>>("api/collectiontweets");
            return MapCollectionTweetDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all collection tweets");
            throw;
        }
    }

    public async Task<List<CollectionTweetDetailViewModel>> GetAllCollectionTweetsWithDetailsAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CollectionTweetDetailDto>>>("api/collectiontweets/with-details");
            return MapCollectionTweetDetailDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all collection tweets with details");
            throw;
        }
    }

    public async Task<CollectionTweetViewModel> GetCollectionTweetByIdAsync(Guid id)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<CollectionTweetDto>>($"api/collectiontweets/{id}");
            return MapCollectionTweetDtoToViewModel(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection tweet with ID {Id}", id);
            throw;
        }
    }

    public async Task<List<CollectionTweetViewModel>> GetCollectionTweetsByCollectionIdAsync(Guid collectionId)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CollectionTweetDto>>>($"api/collectiontweets/by-collection/{collectionId}");
            return MapCollectionTweetDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection tweets by collection ID {CollectionId}", collectionId);
            throw;
        }
    }

    public async Task<List<CollectionTweetViewModel>> GetCollectionTweetsByCollectionIdOrderedAsync(Guid collectionId)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CollectionTweetDto>>>($"api/collectiontweets/by-collection/{collectionId}/ordered");
            return MapCollectionTweetDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ordered collection tweets by collection ID {CollectionId}", collectionId);
            throw;
        }
    }

    public async Task<List<CollectionTweetViewModel>> GetCollectionTweetsByTweetIdAsync(Guid tweetId)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<CollectionTweetDto>>>($"api/collectiontweets/by-tweet/{tweetId}");
            return MapCollectionTweetDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection tweets by tweet ID {TweetId}", tweetId);
            throw;
        }
    }

    public async Task<(List<CollectionTweetViewModel> Items, int TotalCount)> GetPagedCollectionTweetsAsync(int pageIndex, int pageSize)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<(IReadOnlyList<CollectionTweetDto> Items, int TotalCount)>>($"api/collectiontweets/paged?pageIndex={pageIndex}&pageSize={pageSize}");

            return (
                MapCollectionTweetDtosToViewModels(result.Data.Items),
                result.Data.TotalCount
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged collection tweets with page index {PageIndex} and page size {PageSize}", pageIndex, pageSize);
            throw;
        }
    }

    public async Task<CollectionTweetViewModel> CreateCollectionTweetAsync(CreateCollectionTweetViewModel createViewModel)
    {
        try
        {
            var createDto = new CreateCollectionTweetDto
            {
                CollectionId = createViewModel.CollectionId,
                TweetId = createViewModel.TweetId,
                DisplayOrder = createViewModel.DisplayOrder
            };

            var result = await _apiService.PostAsync<CreateCollectionTweetDto, ServiceResult<CollectionTweetDto>>("api/collectiontweets", createDto);
            return MapCollectionTweetDtoToViewModel(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating collection tweet");
            throw;
        }
    }

    public async Task<List<CollectionTweetViewModel>> AssignTweetsToCollectionAsync(Guid collectionId, List<Guid> tweetIds)
    {
        try
        {
            var result = await _apiService.PostAsync<List<Guid>, ServiceResult<IReadOnlyList<CollectionTweetDto>>>($"api/collectiontweets/collection/{collectionId}/assign-tweets", tweetIds);
            return MapCollectionTweetDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning tweets to collection with ID {CollectionId}", collectionId);
            throw;
        }
    }

    public async Task<bool> RemoveTweetFromCollectionAsync(Guid collectionId, Guid tweetId)
    {
        try
        {
            var result = await _apiService.DeleteAsync($"api/collectiontweets/collection/{collectionId}/tweet/{tweetId}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing tweet with ID {TweetId} from collection with ID {CollectionId}", tweetId, collectionId);
            throw;
        }
    }

    public async Task<bool> UpdateTweetDisplayOrderInCollectionAsync(Guid collectionId, Dictionary<Guid, int> tweetOrderUpdates)
    {
        try
        {
            var result = await _apiService.PutAsync<Dictionary<Guid, int>, ServiceResult<bool>>($"api/collectiontweets/collection/{collectionId}/update-order", tweetOrderUpdates);
            return result.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tweet display order in collection with ID {CollectionId}", collectionId);
            throw;
        }
    }

    #region Helper Methods

    private CollectionTweetViewModel MapCollectionTweetDtoToViewModel(CollectionTweetDto dto)
    {
        return dto == null ? null : new CollectionTweetViewModel
        {
            Id = dto.Id,
            CollectionId = dto.CollectionId,
            TweetId = dto.TweetId,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    private List<CollectionTweetViewModel> MapCollectionTweetDtosToViewModels(IEnumerable<CollectionTweetDto> dtos)
    {
        return dtos?.Select(dto => MapCollectionTweetDtoToViewModel(dto)).ToList() ?? new List<CollectionTweetViewModel>();
    }

    private CollectionTweetDetailViewModel MapCollectionTweetDetailDtoToViewModel(CollectionTweetDetailDto dto)
    {
        if (dto == null) return null;

        return new CollectionTweetDetailViewModel
        {
            Id = dto.Id,
            CollectionId = dto.CollectionId,
            TweetId = dto.TweetId,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Collection = dto.Collection != null ? new Models.Collection.CollectionViewModel
            {
                Id = dto.Collection.Id,
                Name = dto.Collection.Name,
                Description = dto.Collection.Description,
                IconUrl = dto.Collection.IconUrl,
                IsPublic = dto.Collection.IsPublic,
                DisplayOrder = dto.Collection.DisplayOrder,
                CreatedAt = dto.Collection.CreatedAt,
                UpdatedAt = dto.Collection.UpdatedAt
            } : null,
            Tweet = dto.Tweet != null ? new TweetViewModel
            {
                Id = dto.Tweet.Id,
                Content = dto.Tweet.Content,
                AuthorUsername = dto.Tweet.AuthorUsername,
                OriginalUrl = dto.Tweet.OriginalUrl,
                TweetDate = dto.Tweet.TweetDate,
                ImageUrl = dto.Tweet.ImageUrl,
                IsSeen = dto.Tweet.IsSeen,
                PlatformName = dto.Tweet.PlatformName,
                CreatedAt = dto.Tweet.CreatedAt,
                UpdatedAt = dto.Tweet.UpdatedAt
            } : null
        };
    }

    private List<CollectionTweetDetailViewModel> MapCollectionTweetDetailDtosToViewModels(IEnumerable<CollectionTweetDetailDto> dtos)
    {
        return dtos?.Select(dto => MapCollectionTweetDetailDtoToViewModel(dto)).ToList() ?? new List<CollectionTweetDetailViewModel>();
    }

    #endregion
}
