using BookMarkBrain.Core.DTOs.TweetHashtag;
using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Implementations;

public class TweetHashtagService : BaseService<TweetHashtag, TweetHashtagDto>, ITweetHashtagService
{
    private readonly ITweetHashtagRepository _tweetHashtagRepository;
    private readonly ITweetRepository _tweetRepository;
    private readonly IHashtagRepository _hashtagRepository;
    private readonly IValidator<CreateTweetHashtagDto> _createValidator;
    private readonly ILogger<TweetHashtagService> _logger;

    public TweetHashtagService(
        ITweetHashtagRepository tweetHashtagRepository,
        ITweetRepository tweetRepository,
        IHashtagRepository hashtagRepository,
        IValidator<CreateTweetHashtagDto> createValidator,
        ILogger<TweetHashtagService> logger)
        : base(tweetHashtagRepository, logger)
    {
        _tweetHashtagRepository = tweetHashtagRepository ?? throw new ArgumentNullException(nameof(tweetHashtagRepository));
        _tweetRepository = tweetRepository ?? throw new ArgumentNullException(nameof(tweetRepository));
        _hashtagRepository = hashtagRepository ?? throw new ArgumentNullException(nameof(hashtagRepository));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<TweetHashtagDto>> CreateTweetHashtagAsync(CreateTweetHashtagDto createDto)
    {
        try
        {
            // Validate the data
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<TweetHashtagDto>.FailureResult(
                    "Validation failed for TweetHashtag creation",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Check if the Tweet exists
            var tweet = await _tweetRepository.GetByIdAsync(createDto.TweetId);
            if (tweet == null)
            {
                return ServiceResult<TweetHashtagDto>.FailureResult($"Tweet with ID {createDto.TweetId} not found");
            }

            // Check if the Hashtag exists
            var hashtag = await _hashtagRepository.GetByIdAsync(createDto.HashtagId);
            if (hashtag == null)
            {
                return ServiceResult<TweetHashtagDto>.FailureResult($"Hashtag with ID {createDto.HashtagId} not found");
            }

            // Check if the relationship already exists
            var existingRelation = await _tweetHashtagRepository.GetByTweetAndHashtagIdAsync(createDto.TweetId, createDto.HashtagId);
            if (existingRelation != null)
            {
                return ServiceResult<TweetHashtagDto>.FailureResult("This Tweet is already tagged with this Hashtag");
            }

            // Create and save the new entity
            var tweetHashtag = new TweetHashtag
            {
                TweetId = createDto.TweetId,
                HashtagId = createDto.HashtagId
            };

            var createdEntity = await _tweetHashtagRepository.AddAsync(tweetHashtag);
            await _tweetHashtagRepository.SaveChangesAsync();

            // Increment hashtag usage count
            await _hashtagRepository.IncrementUsageCountAsync(createDto.HashtagId);

            // Map the result
            var result = createdEntity.Adapt<TweetHashtagDto>();
            result.TweetContent = tweet.Content;
            result.HashtagName = hashtag.Name;

            _logger.LogInformation("Created TweetHashtag relationship: Tweet ID {TweetId} with Hashtag ID {HashtagId}",
                createDto.TweetId, createDto.HashtagId);

            return ServiceResult<TweetHashtagDto>.SuccessResult(result, "Tweet hashtag relationship created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating TweetHashtag relationship");
            return ServiceResult<TweetHashtagDto>.FailureResult($"Error creating TweetHashtag relationship: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByTweetIdAsync(Guid tweetId)
    {
        try
        {
            var tweetHashtags = await _tweetHashtagRepository.GetByTweetIdAsync(tweetId);
            var result = new List<TweetHashtagDto>();

            foreach (var th in tweetHashtags)
            {
                var tweet = await _tweetRepository.GetByIdAsync(th.TweetId);
                var hashtag = await _hashtagRepository.GetByIdAsync(th.HashtagId);

                var dto = th.Adapt<TweetHashtagDto>();
                dto.TweetContent = tweet?.Content;
                dto.HashtagName = hashtag?.Name;

                result.Add(dto);
            }

            return ServiceResult<IReadOnlyList<TweetHashtagDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving TweetHashtags for Tweet ID {TweetId}", tweetId);
            return ServiceResult<IReadOnlyList<TweetHashtagDto>>.FailureResult($"Error retrieving TweetHashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByHashtagIdAsync(Guid hashtagId)
    {
        try
        {
            var tweetHashtags = await _tweetHashtagRepository.GetByHashtagIdAsync(hashtagId);
            var result = new List<TweetHashtagDto>();

            foreach (var th in tweetHashtags)
            {
                var tweet = await _tweetRepository.GetByIdAsync(th.TweetId);
                var hashtag = await _hashtagRepository.GetByIdAsync(th.HashtagId);

                var dto = th.Adapt<TweetHashtagDto>();
                dto.TweetContent = tweet?.Content;
                dto.HashtagName = hashtag?.Name;

                result.Add(dto);
            }

            return ServiceResult<IReadOnlyList<TweetHashtagDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving TweetHashtags for Hashtag ID {HashtagId}", hashtagId);
            return ServiceResult<IReadOnlyList<TweetHashtagDto>>.FailureResult($"Error retrieving TweetHashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetAllWithDetailsAsync()
    {
        try
        {
            var tweetHashtags = await _tweetHashtagRepository.GetAllWithDetailsAsync();
            var result = new List<TweetHashtagWithDetailsDto>();

            foreach (var th in tweetHashtags)
            {
                var dto = new TweetHashtagWithDetailsDto
                {
                    Id = th.Id,
                    TweetId = th.TweetId,
                    HashtagId = th.HashtagId,
                    CreatedAt = th.CreatedAt,
                    UpdatedAt = th.UpdatedAt
                };

                if (th.Tweet != null)
                {
                    dto.TweetContent = th.Tweet.Content;
                    dto.AuthorUsername = th.Tweet.AuthorUsername;
                    dto.OriginalUrl = th.Tweet.OriginalUrl;
                    dto.TweetDate = th.Tweet.TweetDate;
                }

                if (th.Hashtag != null)
                {
                    dto.HashtagName = th.Hashtag.Name;
                    dto.HashtagDescription = th.Hashtag.Description;
                    dto.IsPopular = th.Hashtag.IsPopular;
                    dto.UsageCount = th.Hashtag.UsageCount;
                }

                result.Add(dto);
            }

            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all TweetHashtags with details");
            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.FailureResult($"Error retrieving TweetHashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByTweetIdWithDetailsAsync(Guid tweetId)
    {
        try
        {
            var tweetHashtags = await _tweetHashtagRepository.GetByTweetIdWithDetailsAsync(tweetId);
            var result = new List<TweetHashtagWithDetailsDto>();

            foreach (var th in tweetHashtags)
            {
                var dto = new TweetHashtagWithDetailsDto
                {
                    Id = th.Id,
                    TweetId = th.TweetId,
                    HashtagId = th.HashtagId,
                    CreatedAt = th.CreatedAt,
                    UpdatedAt = th.UpdatedAt
                };

                if (th.Tweet != null)
                {
                    dto.TweetContent = th.Tweet.Content;
                    dto.AuthorUsername = th.Tweet.AuthorUsername;
                    dto.OriginalUrl = th.Tweet.OriginalUrl;
                    dto.TweetDate = th.Tweet.TweetDate;
                }

                if (th.Hashtag != null)
                {
                    dto.HashtagName = th.Hashtag.Name;
                    dto.HashtagDescription = th.Hashtag.Description;
                    dto.IsPopular = th.Hashtag.IsPopular;
                    dto.UsageCount = th.Hashtag.UsageCount;
                }

                result.Add(dto);
            }

            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving TweetHashtags with details for Tweet ID {TweetId}", tweetId);
            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.FailureResult($"Error retrieving TweetHashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByHashtagIdWithDetailsAsync(Guid hashtagId)
    {
        try
        {
            var tweetHashtags = await _tweetHashtagRepository.GetByHashtagIdWithDetailsAsync(hashtagId);
            var result = new List<TweetHashtagWithDetailsDto>();

            foreach (var th in tweetHashtags)
            {
                var dto = new TweetHashtagWithDetailsDto
                {
                    Id = th.Id,
                    TweetId = th.TweetId,
                    HashtagId = th.HashtagId,
                    CreatedAt = th.CreatedAt,
                    UpdatedAt = th.UpdatedAt
                };

                if (th.Tweet != null)
                {
                    dto.TweetContent = th.Tweet.Content;
                    dto.AuthorUsername = th.Tweet.AuthorUsername;
                    dto.OriginalUrl = th.Tweet.OriginalUrl;
                    dto.TweetDate = th.Tweet.TweetDate;
                }

                if (th.Hashtag != null)
                {
                    dto.HashtagName = th.Hashtag.Name;
                    dto.HashtagDescription = th.Hashtag.Description;
                    dto.IsPopular = th.Hashtag.IsPopular;
                    dto.UsageCount = th.Hashtag.UsageCount;
                }

                result.Add(dto);
            }

            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving TweetHashtags with details for Hashtag ID {HashtagId}", hashtagId);
            return ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>.FailureResult($"Error retrieving TweetHashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> RemoveTweetHashtagAsync(Guid tweetId, Guid hashtagId)
    {
        try
        {
            var tweetHashtag = await _tweetHashtagRepository.GetByTweetAndHashtagIdAsync(tweetId, hashtagId);
            if (tweetHashtag == null)
            {
                return ServiceResult<bool>.FailureResult($"Relationship between Tweet ID {tweetId} and Hashtag ID {hashtagId} not found");
            }

            await _tweetHashtagRepository.DeleteAsync(tweetHashtag);
            await _tweetHashtagRepository.SaveChangesAsync();

            _logger.LogInformation("Removed TweetHashtag relationship: Tweet ID {TweetId} with Hashtag ID {HashtagId}",
                tweetId, hashtagId);

            return ServiceResult<bool>.SuccessResult(true, "TweetHashtag relationship removed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing TweetHashtag relationship for Tweet ID {TweetId} and Hashtag ID {HashtagId}",
                tweetId, hashtagId);
            return ServiceResult<bool>.FailureResult($"Error removing TweetHashtag relationship: {ex.Message}");
        }
    }
}