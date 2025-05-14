using BookMarkBrain.Core.DTOs.TweetCategory;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Models.TweetCategory;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceImplementations;

public class TweetCategoryApiService : ITweetCategoryApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<TweetCategoryApiService> _logger;
    private const string BaseEndpoint = "api/TweetCategory";

    public TweetCategoryApiService(IApiService apiService, ILogger<TweetCategoryApiService> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TweetCategoryViewModel> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _apiService.GetAsync<ServiceResult<TweetCategoryDto>>($"{BaseEndpoint}/{id}");
            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet category with ID {Id}", id);
            throw;
        }
    }

    public async Task<List<TweetCategoryViewModel>> GetAllAsync()
    {
        try
        {
            var response = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetCategoryDto>>>($"{BaseEndpoint}");
            return response.Data.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tweet categories");
            throw;
        }
    }

    public async Task<List<TweetCategoryViewModel>> GetByTweetIdAsync(Guid tweetId)
    {
        try
        {
            var response = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetCategoryDto>>>($"{BaseEndpoint}/tweet/{tweetId}");
            return response.Data.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet categories for tweet with ID {TweetId}", tweetId);
            throw;
        }
    }

    public async Task<List<TweetCategoryViewModel>> GetByCategoryIdAsync(Guid categoryId)
    {
        try
        {
            var response = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetCategoryDto>>>($"{BaseEndpoint}/category/{categoryId}");
            return response.Data.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet categories for category with ID {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<(List<TweetCategoryViewModel> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize)
    {
        try
        {
            var response = await _apiService.GetAsync<ServiceResult<(IReadOnlyList<TweetCategoryDto> Items, int TotalCount)>>(
                $"{BaseEndpoint}/paged?pageIndex={pageIndex}&pageSize={pageSize}");

            var items = response.Data.Items.Select(MapToViewModel).ToList();
            return (items, response.Data.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged tweet categories");
            throw;
        }
    }

    public async Task<TweetCategoryViewModel> CreateAsync(TweetCategoryCreateViewModel viewModel)
    {
        try
        {
            var dto = new CreateTweetCategoryDto
            {
                TweetId = viewModel.TweetId,
                CategoryId = viewModel.CategoryId
            };

            var response = await _apiService.PostAsync<CreateTweetCategoryDto, ServiceResult<TweetCategoryDto>>(
                BaseEndpoint, dto);

            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tweet category");
            throw;
        }
    }

    public async Task<TweetCategoryViewModel> UpdateAsync(TweetCategoryUpdateViewModel viewModel)
    {
        try
        {
            var dto = new UpdateTweetCategoryDto
            {
                TweetId = viewModel.TweetId,
                CategoryId = viewModel.CategoryId
            };

            var response = await _apiService.PutAsync<UpdateTweetCategoryDto, ServiceResult<TweetCategoryDto>>(
                $"{BaseEndpoint}/{viewModel.Id}", dto);

            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tweet category with ID {Id}", viewModel.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tweet category with ID {Id}", id);
            throw;
        }
    }

    public async Task<List<TweetCategoryViewModel>> AssignCategoriesToTweetAsync(Guid tweetId, List<Guid> categoryIds)
    {
        try
        {
            var response = await _apiService.PostAsync<List<Guid>, ServiceResult<IReadOnlyList<TweetCategoryDto>>>(
                $"{BaseEndpoint}/tweet/{tweetId}/assign-categories", categoryIds);

            return response.Data.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning categories to tweet with ID {TweetId}", tweetId);
            throw;
        }
    }

    public async Task<bool> RemoveCategoryFromTweetAsync(Guid tweetId, Guid categoryId)
    {
        try
        {
            var response = await _apiService.DeleteAsync($"{BaseEndpoint}/tweet/{tweetId}/category/{categoryId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing category {CategoryId} from tweet {TweetId}", categoryId, tweetId);
            throw;
        }
    }

    private TweetCategoryViewModel MapToViewModel(TweetCategoryDto dto)
    {
        if (dto == null) return null;

        return new TweetCategoryViewModel
        {
            Id = dto.Id,
            TweetId = dto.TweetId,
            CategoryId = dto.CategoryId,
            TweetContent = dto.TweetContent,
            CategoryName = dto.CategoryName,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}