using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceImplementations;

public class HashtagApiService : IHashtagApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<HashtagApiService> _logger;
    private const string BaseEndpoint = "api/hashtag";

    public HashtagApiService(IApiService apiService, ILogger<HashtagApiService> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<IReadOnlyList<HashtagDto>>> GetAllHashtagsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all hashtags from API");
            return await _apiService.GetAsync<ServiceResult<IReadOnlyList<HashtagDto>>>(BaseEndpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all hashtags from API");
            return ServiceResult<IReadOnlyList<HashtagDto>>.FailureResult($"Error fetching hashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<HashtagDto>> GetHashtagByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Fetching hashtag with ID {HashtagId} from API", id);
            return await _apiService.GetAsync<ServiceResult<HashtagDto>>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hashtag with ID {HashtagId} from API", id);
            return ServiceResult<HashtagDto>.FailureResult($"Error fetching hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<HashtagDto>> GetHashtagByNameAsync(string name)
    {
        try
        {
            _logger.LogInformation("Fetching hashtag with name {HashtagName} from API", name);
            return await _apiService.GetAsync<ServiceResult<HashtagDto>>($"{BaseEndpoint}/by-name/{name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hashtag with name {HashtagName} from API", name);
            return ServiceResult<HashtagDto>.FailureResult($"Error fetching hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<HashtagDto>>> GetPopularHashtagsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching popular hashtags from API");
            return await _apiService.GetAsync<ServiceResult<IReadOnlyList<HashtagDto>>>($"{BaseEndpoint}/popular");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching popular hashtags from API");
            return ServiceResult<IReadOnlyList<HashtagDto>>.FailureResult($"Error fetching popular hashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<HashtagDto>> CreateHashtagAsync(CreateHashtagDto createHashtagDto)
    {
        try
        {
            _logger.LogInformation("Creating new hashtag with name {HashtagName} through API", createHashtagDto.Name);
            return await _apiService.PostAsync<CreateHashtagDto, ServiceResult<HashtagDto>>(BaseEndpoint, createHashtagDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hashtag with name {HashtagName} through API", createHashtagDto.Name);
            return ServiceResult<HashtagDto>.FailureResult($"Error creating hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<HashtagDto>> UpdateHashtagAsync(Guid id, UpdateHashtagDto updateHashtagDto)
    {
        try
        {
            _logger.LogInformation("Updating hashtag with ID {HashtagId} through API", id);
            return await _apiService.PutAsync<UpdateHashtagDto, ServiceResult<HashtagDto>>($"{BaseEndpoint}/{id}", updateHashtagDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hashtag with ID {HashtagId} through API", id);
            return ServiceResult<HashtagDto>.FailureResult($"Error updating hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteHashtagAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting hashtag with ID {HashtagId} through API", id);
            var success = await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            return ServiceResult<bool>.SuccessResult(success, "Hashtag deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hashtag with ID {HashtagId} through API", id);
            return ServiceResult<bool>.FailureResult($"Error deleting hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<int>> IncrementUsageCountAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Incrementing usage count for hashtag ID {HashtagId} through API", id);
            return await _apiService.GetAsync<ServiceResult<int>>($"{BaseEndpoint}/{id}/increment-usage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing usage count for hashtag ID {HashtagId} through API", id);
            return ServiceResult<int>.FailureResult($"Error incrementing usage count: {ex.Message}");
        }
    }
}
