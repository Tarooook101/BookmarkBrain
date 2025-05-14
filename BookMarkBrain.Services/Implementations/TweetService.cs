using BookMarkBrain.Core.DTOs.Tweet;
using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using BookMarkBrain.Services.Mappings;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Implementations;

public class TweetService : BaseService<Tweet, TweetDto>, ITweetService
{
    private readonly ITweetRepository _tweetRepository;
    private readonly HtmlParserHelper _htmlParserHelper;
    private readonly IValidator<CreateTweetDto> _createValidator;
    private readonly IValidator<UpdateTweetDto> _updateValidator;

    public TweetService(
        ITweetRepository tweetRepository,
        HtmlParserHelper htmlParserHelper,
        IValidator<CreateTweetDto> createValidator,
        IValidator<UpdateTweetDto> updateValidator,
        ILogger<TweetService> logger)
        : base(tweetRepository, logger)
    {
        _tweetRepository = tweetRepository ?? throw new ArgumentNullException(nameof(tweetRepository));
        _htmlParserHelper = htmlParserHelper ?? throw new ArgumentNullException(nameof(htmlParserHelper));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
    }

    public override async Task<ServiceResult<TweetDto>> CreateAsync(TweetDto entityDto)
    {
        try
        {
            // Convert to CreateTweetDto for validation
            var createDto = entityDto.Adapt<CreateTweetDto>();

            // Validate
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<TweetDto>.FailureResult(
                    "Tweet validation failed",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Create entity and save
            var entity = entityDto.Adapt<Tweet>();
            var createdEntity = await _tweetRepository.AddAsync(entity);
            await _tweetRepository.SaveChangesAsync();

            var result = createdEntity.Adapt<TweetDto>();
            return ServiceResult<TweetDto>.SuccessResult(result, "Tweet created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tweet");
            return ServiceResult<TweetDto>.FailureResult($"Error creating tweet: {ex.Message}");
        }
    }

    public override async Task<ServiceResult<TweetDto>> UpdateAsync(Guid id, TweetDto entityDto)
    {
        try
        {
            // Convert to UpdateTweetDto for validation
            var updateDto = entityDto.Adapt<UpdateTweetDto>();

            // Validate
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<TweetDto>.FailureResult(
                    "Tweet validation failed",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var existingEntity = await _tweetRepository.GetByIdAsync(id);
            if (existingEntity == null)
                return ServiceResult<TweetDto>.FailureResult($"Tweet with ID {id} not found");

            // Map properties from DTO to existing entity
            entityDto.Adapt(existingEntity);

            await _tweetRepository.UpdateAsync(existingEntity);
            await _tweetRepository.SaveChangesAsync();

            var result = existingEntity.Adapt<TweetDto>();
            return ServiceResult<TweetDto>.SuccessResult(result, "Tweet updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tweet with ID {Id}", id);
            return ServiceResult<TweetDto>.FailureResult($"Error updating tweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetDto>>> GetTweetsByCategoryIdAsync(Guid categoryId)
    {
        try
        {
            var tweets = await _tweetRepository.GetTweetsByCategoryIdAsync(categoryId);
            var result = tweets.Adapt<IReadOnlyList<TweetDto>>();
            return ServiceResult<IReadOnlyList<TweetDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweets for category with ID {CategoryId}", categoryId);
            return ServiceResult<IReadOnlyList<TweetDto>>.FailureResult($"Error getting tweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetDto>>> GetTweetsByPlatformAsync(string platformName)
    {
        try
        {
            var tweets = await _tweetRepository.GetTweetsByPlatformAsync(platformName);
            var result = tweets.Adapt<IReadOnlyList<TweetDto>>();
            return ServiceResult<IReadOnlyList<TweetDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweets for platform {PlatformName}", platformName);
            return ServiceResult<IReadOnlyList<TweetDto>>.FailureResult($"Error getting tweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<TweetDto>> ToggleSeenStatusAsync(Guid id)
    {
        try
        {
            var tweet = await _tweetRepository.GetByIdAsync(id);
            if (tweet == null)
                return ServiceResult<TweetDto>.FailureResult($"Tweet with ID {id} not found");

            tweet.IsSeen = !tweet.IsSeen;
            await _tweetRepository.UpdateAsync(tweet);
            await _tweetRepository.SaveChangesAsync();

            var result = tweet.Adapt<TweetDto>();
            return ServiceResult<TweetDto>.SuccessResult(result, $"Tweet marked as {(tweet.IsSeen ? "seen" : "unseen")}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling seen status for tweet with ID {Id}", id);
            return ServiceResult<TweetDto>.FailureResult($"Error updating tweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<TweetDto>> ExtractTweetFromUrlAsync(string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                return ServiceResult<TweetDto>.FailureResult("URL cannot be empty");

            // Extract content using HtmlAgilityPack
            var content = await _htmlParserHelper.ExtractTweetContentAsync(url);

            if (string.IsNullOrWhiteSpace(content))
                return ServiceResult<TweetDto>.FailureResult("Could not extract content from the provided URL");

            // Create a new tweet with extracted content
            var tweet = new Tweet
            {
                Content = content,
                OriginalUrl = url,
                AuthorUsername = "Unknown", // This could be improved with better extraction
                TweetDate = DateTime.UtcNow,
                IsSeen = false,
                PlatformName = "Twitter" // Assuming Twitter by default
            };

            var createdEntity = await _tweetRepository.AddAsync(tweet);
            await _tweetRepository.SaveChangesAsync();

            var result = createdEntity.Adapt<TweetDto>();
            return ServiceResult<TweetDto>.SuccessResult(result, "Tweet extracted and created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting tweet from URL: {Url}", url);
            return ServiceResult<TweetDto>.FailureResult($"Error extracting tweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetDto>>> SearchTweetsAsync(string searchTerm)
    {
        try
        {
            var tweets = await _tweetRepository.SearchTweetsAsync(searchTerm);
            var result = tweets.Adapt<IReadOnlyList<TweetDto>>();
            return ServiceResult<IReadOnlyList<TweetDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tweets with term: {SearchTerm}", searchTerm);
            return ServiceResult<IReadOnlyList<TweetDto>>.FailureResult($"Error searching tweets: {ex.Message}");
        }
    }
}