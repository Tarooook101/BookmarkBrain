using BookMarkBrain.Core.DTOs.Tweet;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.MVC.Models.Tweet;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceImplementations;

public class TweetApiService : ITweetApiService
{
    private readonly IApiService _apiService;
    private readonly ILogger<TweetApiService> _logger;
    private const string BaseEndpoint = "api/tweet";

    public TweetApiService(IApiService apiService, ILogger<TweetApiService> logger)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<TweetViewModel>> GetAllTweetsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all tweets");
            var response = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetDto>>>(BaseEndpoint);

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to fetch tweets: {Message}", response.Message);
                return new List<TweetViewModel>();
            }

            // Map DTOs to ViewModels
            return response.Data.Select(t => MapToViewModel(t)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching all tweets");
            throw;
        }
    }

    public async Task<TweetViewModel> GetTweetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Fetching tweet with ID: {TweetId}", id);
            var response = await _apiService.GetAsync<ServiceResult<TweetDto>>($"{BaseEndpoint}/{id}");

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to fetch tweet with ID {TweetId}: {Message}", id, response.Message);
                return null;
            }

            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching tweet with ID: {TweetId}", id);
            throw;
        }
    }

    public async Task<List<TweetViewModel>> GetTweetsByCategoryAsync(Guid categoryId)
    {
        try
        {
            _logger.LogInformation("Fetching tweets for category with ID: {CategoryId}", categoryId);
            var response = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetDto>>>($"{BaseEndpoint}/category/{categoryId}");

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to fetch tweets for category with ID {CategoryId}: {Message}",
                    categoryId, response.Message);
                return new List<TweetViewModel>();
            }

            return response.Data.Select(t => MapToViewModel(t)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching tweets for category with ID: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<List<TweetViewModel>> GetTweetsByPlatformAsync(string platformName)
    {
        try
        {
            _logger.LogInformation("Fetching tweets for platform: {PlatformName}", platformName);
            var response = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetDto>>>($"{BaseEndpoint}/platform/{platformName}");

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to fetch tweets for platform {PlatformName}: {Message}",
                    platformName, response.Message);
                return new List<TweetViewModel>();
            }

            return response.Data.Select(t => MapToViewModel(t)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching tweets for platform: {PlatformName}", platformName);
            throw;
        }
    }

    public async Task<List<TweetViewModel>> SearchTweetsAsync(string searchTerm)
    {
        try
        {
            _logger.LogInformation("Searching tweets with term: {SearchTerm}", searchTerm);
            var response = await _apiService.GetAsync<ServiceResult<IReadOnlyList<TweetDto>>>($"{BaseEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}");

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to search tweets with term {SearchTerm}: {Message}",
                    searchTerm, response.Message);
                return new List<TweetViewModel>();
            }

            return response.Data.Select(t => MapToViewModel(t)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while searching tweets with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<TweetViewModel> CreateTweetAsync(CreateTweetViewModel tweetViewModel)
    {
        try
        {
            _logger.LogInformation("Creating new tweet");

            // Map ViewModel to DTO
            var tweetDto = new TweetDto
            {
                Content = tweetViewModel.Content,
                AuthorUsername = tweetViewModel.AuthorUsername,
                OriginalUrl = tweetViewModel.OriginalUrl,
                TweetDate = tweetViewModel.TweetDate,
                ImageUrl = tweetViewModel.ImageUrl,
                IsSeen = tweetViewModel.IsSeen,
                PlatformName = tweetViewModel.PlatformName,
                CategoryId = tweetViewModel.CategoryId
                // Note: Hashtags are handled separately in the future
            };

            var response = await _apiService.PostAsync<TweetDto, ServiceResult<TweetDto>>(BaseEndpoint, tweetDto);

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to create tweet: {Message}", response.Message);
                return null;
            }

            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating new tweet");
            throw;
        }
    }

    public async Task<TweetViewModel> ExtractTweetFromUrlAsync(string url)
    {
        try
        {
            _logger.LogInformation("Extracting tweet from URL: {Url}", url);

            var urlDto = new { Url = url };
            var response = await _apiService.PostAsync<object, ServiceResult<TweetDto>>($"{BaseEndpoint}/extract", urlDto);

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to extract tweet from URL {Url}: {Message}", url, response.Message);
                return null;
            }

            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while extracting tweet from URL: {Url}", url);
            throw;
        }
    }

    public async Task<TweetViewModel> UpdateTweetAsync(UpdateTweetViewModel tweetViewModel)
    {
        try
        {
            _logger.LogInformation("Updating tweet with ID: {TweetId}", tweetViewModel.Id);

            // Map ViewModel to DTO
            var tweetDto = new TweetDto
            {
                Id = tweetViewModel.Id,
                Content = tweetViewModel.Content,
                AuthorUsername = tweetViewModel.AuthorUsername,
                OriginalUrl = tweetViewModel.OriginalUrl,
                TweetDate = tweetViewModel.TweetDate,
                ImageUrl = tweetViewModel.ImageUrl,
                IsSeen = tweetViewModel.IsSeen,
                PlatformName = tweetViewModel.PlatformName,
                CategoryId = tweetViewModel.CategoryId
                // Note: Hashtags are handled separately in the future
            };

            var response = await _apiService.PutAsync<TweetDto, ServiceResult<TweetDto>>($"{BaseEndpoint}/{tweetViewModel.Id}", tweetDto);

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to update tweet with ID {TweetId}: {Message}",
                    tweetViewModel.Id, response.Message);
                return null;
            }

            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating tweet with ID: {TweetId}", tweetViewModel.Id);
            throw;
        }
    }

    public async Task<TweetViewModel> ToggleTweetSeenStatusAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Toggling seen status for tweet with ID: {TweetId}", id);

            var response = await _apiService.PutAsync<object, ServiceResult<TweetDto>>($"{BaseEndpoint}/{id}/toggle-seen", null);

            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to toggle seen status for tweet with ID {TweetId}: {Message}",
                    id, response.Message);
                return null;
            }

            return MapToViewModel(response.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while toggling seen status for tweet with ID: {TweetId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTweetAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting tweet with ID: {TweetId}", id);

            var response = await _apiService.DeleteAsync($"{BaseEndpoint}/{id}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting tweet with ID: {TweetId}", id);
            throw;
        }
    }

    // Helper method to map DTO to ViewModel
    private TweetViewModel MapToViewModel(TweetDto tweetDto)
    {
        if (tweetDto == null)
            return null;

        return new TweetViewModel
        {
            Id = tweetDto.Id,
            Content = tweetDto.Content,
            AuthorUsername = tweetDto.AuthorUsername,
            OriginalUrl = tweetDto.OriginalUrl,
            TweetDate = tweetDto.TweetDate,
            ImageUrl = tweetDto.ImageUrl,
            IsSeen = tweetDto.IsSeen,
            PlatformName = tweetDto.PlatformName,
            CategoryId = tweetDto.CategoryId,
            CreatedAt = tweetDto.CreatedAt,
            UpdatedAt = tweetDto.UpdatedAt,
            // CategoryName and Hashtags will be populated separately when we implement those entities
        };
    }
}
