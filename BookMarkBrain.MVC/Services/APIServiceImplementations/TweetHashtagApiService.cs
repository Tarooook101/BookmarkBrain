using BookMarkBrain.Core.DTOs.TweetHashtag;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceImplementations;


public class TweetHashtagApiService : ITweetHashtagApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<TweetHashtagApiService> _logger;
    private const string BaseEndpoint = "api/TweetHashtag";

    public TweetHashtagApiService(IApiService apiService, ILogger<TweetHashtagApiService> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetAllTweetHashtagsAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetHashtagDto>>>(BaseEndpoint);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tweet hashtags");
            return ServiceResult<IReadOnlyList<TweetHashtagDto>>.FailureResult($"Error retrieving tweet hashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<TweetHashtagDto>> GetTweetHashtagByIdAsync(Guid id)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<TweetHashtagDto>>($"{BaseEndpoint}/{id}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtag with ID {TweetHashtagId}", id);
            return ServiceResult<TweetHashtagDto>.FailureResult($"Error retrieving tweet hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByTweetIdAsync(Guid tweetId)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetHashtagDto>>>($"{BaseEndpoint}/tweet/{tweetId}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtags for tweet ID {TweetId}", tweetId);
            return ServiceResult<IReadOnlyList<TweetHashtagDto>>.FailureResult($"Error retrieving tweet hashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByHashtagIdAsync(Guid hashtagId)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetHashtagDto>>>($"{BaseEndpoint}/hashtag/{hashtagId}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtags for hashtag ID {HashtagId}", hashtagId);
            return ServiceResult<IReadOnlyList<TweetHashtagDto>>.FailureResult($"Error retrieving tweet hashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<TweetHashtagDto>> CreateTweetHashtagAsync(CreateTweetHashtagDto createTweetHashtagDto)
    {
        try
        {
            var result = await _apiService.PostAsync<CreateTweetHashtagDto, ServiceResult<TweetHashtagDto>>(BaseEndpoint, createTweetHashtagDto);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tweet hashtag relationship");
            return ServiceResult<TweetHashtagDto>.FailureResult($"Error creating tweet hashtag relationship: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteTweetHashtagAsync(Guid id)
    {
        try
        {
            var result = await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            return ServiceResult<bool>.SuccessResult(result, "Tweet hashtag relationship successfully deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tweet hashtag with ID {TweetHashtagId}", id);
            return ServiceResult<bool>.FailureResult($"Error deleting tweet hashtag relationship: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> RemoveTweetHashtagAsync(Guid tweetId, Guid hashtagId)
    {
        try
        {
            var result = await _apiService.DeleteAsync($"{BaseEndpoint}/tweet/{tweetId}/hashtag/{hashtagId}");
            return ServiceResult<bool>.SuccessResult(result, "Tweet hashtag relationship successfully removed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing hashtag ID {HashtagId} from tweet ID {TweetId}", hashtagId, tweetId);
            return ServiceResult<bool>.FailureResult($"Error removing tweet hashtag relationship: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetAllWithDetailsAsync()
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>>($"{BaseEndpoint}/with-details");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tweet hashtags with details");
            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.FailureResult($"Error retrieving tweet hashtags with details: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByTweetIdWithDetailsAsync(Guid tweetId)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>>($"{BaseEndpoint}/tweet/{tweetId}/with-details");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtags with details for tweet ID {TweetId}", tweetId);
            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.FailureResult($"Error retrieving tweet hashtags with details: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByHashtagIdWithDetailsAsync(Guid hashtagId)
    {
        try
        {
            var result = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>>($"{BaseEndpoint}/hashtag/{hashtagId}/with-details");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tweet hashtags with details for hashtag ID {HashtagId}", hashtagId);
            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.FailureResult($"Error retrieving tweet hashtags with details: {ex.Message}");
        }
    }
}