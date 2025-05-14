using BookMarkBrain.Core.DTOs.Collection;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Models.Collection;
using BookMarkBrain.MVC.Models.Tweet;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceImplementations;

public class CollectionApiService : ICollectionApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<CollectionApiService> _logger;

    public CollectionApiService(IApiService apiService, ILogger<CollectionApiService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<List<CollectionViewModel>> GetAllCollectionsAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<List<CollectionDto>>>("api/collections");
            return MapCollectionDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all collections");
            throw;
        }
    }

    public async Task<List<CollectionViewModel>> GetOrderedCollectionsAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<List<CollectionDto>>>("api/collections/ordered");
            return MapCollectionDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ordered collections");
            throw;
        }
    }

    public async Task<CollectionViewModel> GetCollectionByIdAsync(Guid id)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<CollectionDto>>($"api/collections/{id}");
            return MapCollectionDtoToViewModel(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection with ID {Id}", id);
            throw;
        }
    }

    public async Task<CollectionDetailViewModel> GetCollectionWithTweetsAsync(Guid id)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<CollectionDetailDto>>($"api/collections/{id}/with-tweets");
            return MapCollectionDetailDtoToViewModel(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection with tweets for ID {Id}", id);
            throw;
        }
    }

    public async Task<List<CollectionViewModel>> GetPublicCollectionsAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<List<CollectionDto>>>("api/collections/public");
            return MapCollectionDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting public collections");
            throw;
        }
    }

    public async Task<List<CollectionViewModel>> SearchCollectionsAsync(string searchTerm)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<List<CollectionDto>>>($"api/collections/search?term={searchTerm}");
            return MapCollectionDtosToViewModels(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching collections with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<CollectionViewModel> CreateCollectionAsync(CreateCollectionViewModel createViewModel)
    {
        try
        {
            var createDto = new CreateCollectionDto
            {
                Name = createViewModel.Name,
                Description = createViewModel.Description,
                IconUrl = createViewModel.IconUrl,
                IsPublic = createViewModel.IsPublic,
                DisplayOrder = createViewModel.DisplayOrder
            };

            var result = await _apiService.PostAsync<CreateCollectionDto, ServiceResult<CollectionDto>>("api/collections", createDto);
            return MapCollectionDtoToViewModel(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating collection");
            throw;
        }
    }

    public async Task<CollectionViewModel> UpdateCollectionAsync(Guid id, UpdateCollectionViewModel updateViewModel)
    {
        try
        {
            var updateDto = new UpdateCollectionDto
            {
                Name = updateViewModel.Name,
                Description = updateViewModel.Description,
                IconUrl = updateViewModel.IconUrl,
                IsPublic = updateViewModel.IsPublic,
                DisplayOrder = updateViewModel.DisplayOrder
            };

            var result = await _apiService.PutAsync<UpdateCollectionDto, ServiceResult<CollectionDto>>($"api/collections/{id}", updateDto);
            return MapCollectionDtoToViewModel(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating collection with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> UpdateCollectionDisplayOrderAsync(Dictionary<Guid, int> orderUpdates)
    {
        try
        {
            var result = await _apiService.PutAsync<Dictionary<Guid, int>, ServiceResult<bool>>("api/collections/update-order", orderUpdates);
            return result.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating collection display order");
            throw;
        }
    }

    public async Task<bool> DeleteCollectionAsync(Guid id)
    {
        try
        {
            var result = await _apiService.DeleteAsync($"api/collections/{id}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting collection with ID {Id}", id);
            throw;
        }
    }

    #region Helper Methods

    private CollectionViewModel MapCollectionDtoToViewModel(CollectionDto dto)
    {
        return new CollectionViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IconUrl = dto.IconUrl,
            IsPublic = dto.IsPublic,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    private List<CollectionViewModel> MapCollectionDtosToViewModels(List<CollectionDto> dtos)
    {
        return dtos?.Select(dto => MapCollectionDtoToViewModel(dto)).ToList() ?? new List<CollectionViewModel>();
    }

    private CollectionDetailViewModel MapCollectionDetailDtoToViewModel(CollectionDetailDto dto)
    {
        return new CollectionDetailViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IconUrl = dto.IconUrl,
            IsPublic = dto.IsPublic,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Tweets = dto.Tweets?.Select(t => new TweetViewModel
            {
                Id = t.Id,
                Content = t.Content,
                AuthorUsername = t.AuthorUsername,
                OriginalUrl = t.OriginalUrl,
                TweetDate = t.TweetDate,
                ImageUrl = t.ImageUrl,
                IsSeen = t.IsSeen,
                PlatformName = t.PlatformName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList() ?? new List<TweetViewModel>()
        };
    }

    #endregion
}