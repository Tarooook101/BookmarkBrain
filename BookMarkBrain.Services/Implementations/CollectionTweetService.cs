
using BookMarkBrain.Core.DTOs.CollectionTweet;
using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Implementations;


public class CollectionTweetService : ICollectionTweetService
{
    private readonly ICollectionTweetRepository _collectionTweetRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ITweetRepository _tweetRepository;
    private readonly IValidator<CreateCollectionTweetDto> _createValidator;
    private readonly IValidator<UpdateCollectionTweetDto> _updateValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<CollectionTweetService> _logger;

    public CollectionTweetService(
        ICollectionTweetRepository collectionTweetRepository,
        ICollectionRepository collectionRepository,
        ITweetRepository tweetRepository,
        IValidator<CreateCollectionTweetDto> createValidator,
        IValidator<UpdateCollectionTweetDto> updateValidator,
        IMapper mapper,
        ILogger<CollectionTweetService> logger)
    {
        _collectionTweetRepository = collectionTweetRepository ?? throw new ArgumentNullException(nameof(collectionTweetRepository));
        _collectionRepository = collectionRepository ?? throw new ArgumentNullException(nameof(collectionRepository));
        _tweetRepository = tweetRepository ?? throw new ArgumentNullException(nameof(tweetRepository));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<CollectionTweetDto>> CreateAsync(CollectionTweetDto entityDto)
    {
        try
        {
            var entity = _mapper.Map<CollectionTweet>(entityDto);
            var result = await _collectionTweetRepository.AddAsync(entity);
            await _collectionTweetRepository.SaveChangesAsync();

            return ServiceResult<CollectionTweetDto>.SuccessResult(_mapper.Map<CollectionTweetDto>(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating CollectionTweet");
            return ServiceResult<CollectionTweetDto>.FailureResult($"Error occurred while creating CollectionTweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionTweetDto>> CreateCollectionTweetAsync(CreateCollectionTweetDto createDto)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<CollectionTweetDto>.FailureResult(
                    "Validation failed",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Verify both collection and tweet exist
            var collection = await _collectionRepository.GetByIdAsync(createDto.CollectionId);
            if (collection == null)
            {
                return ServiceResult<CollectionTweetDto>.FailureResult($"Collection with id {createDto.CollectionId} not found");
            }

            var tweet = await _tweetRepository.GetByIdAsync(createDto.TweetId);
            if (tweet == null)
            {
                return ServiceResult<CollectionTweetDto>.FailureResult($"Tweet with id {createDto.TweetId} not found");
            }

            // Check if the relationship already exists
            var existing = await _collectionTweetRepository.GetByCollectionAndTweetIdAsync(
                createDto.CollectionId, createDto.TweetId);

            if (existing != null)
            {
                return ServiceResult<CollectionTweetDto>.FailureResult(
                    $"Tweet is already in collection");
            }

            var entity = _mapper.Map<CollectionTweet>(createDto);
            var result = await _collectionTweetRepository.AddAsync(entity);
            await _collectionTweetRepository.SaveChangesAsync();

            return ServiceResult<CollectionTweetDto>.SuccessResult(_mapper.Map<CollectionTweetDto>(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating CollectionTweet");
            return ServiceResult<CollectionTweetDto>.FailureResult($"Error occurred while creating CollectionTweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _collectionTweetRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<bool>.FailureResult($"CollectionTweet with id {id} not found");
            }

            await _collectionTweetRepository.DeleteAsync(entity);
            await _collectionTweetRepository.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting CollectionTweet with id {Id}", id);
            return ServiceResult<bool>.FailureResult($"Error occurred while deleting CollectionTweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _collectionTweetRepository.GetAllAsync();
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionTweetDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all CollectionTweets");
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.FailureResult($"Error occurred while getting CollectionTweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionTweetDetailDto>>> GetAllWithDetailsAsync()
    {
        try
        {
            var entities = await _collectionTweetRepository.GetAllWithDetailsAsync();
            return ServiceResult<IReadOnlyList<CollectionTweetDetailDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionTweetDetailDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all CollectionTweets with details");
            return ServiceResult<IReadOnlyList<CollectionTweetDetailDto>>.FailureResult($"Error occurred while getting CollectionTweets with details: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> AssignTweetsToCollectionAsync(Guid collectionId, List<Guid> tweetIds)
    {
        try
        {
            // Verify collection exists
            var collection = await _collectionRepository.GetByIdAsync(collectionId);
            if (collection == null)
            {
                return ServiceResult<IReadOnlyList<CollectionTweetDto>>.FailureResult($"Collection with id {collectionId} not found");
            }

            var addedEntities = new List<CollectionTweet>();
            var maxOrder = 0;

            // Get existing collection-tweets to determine next display order
            var existingItems = await _collectionTweetRepository.GetByCollectionIdOrderedByDisplayOrderAsync(collectionId);
            if (existingItems.Any())
            {
                maxOrder = existingItems.Max(ct => ct.DisplayOrder);
            }

            foreach (var tweetId in tweetIds)
            {
                // Verify tweet exists
                var tweet = await _tweetRepository.GetByIdAsync(tweetId);
                if (tweet == null)
                {
                    _logger.LogWarning("Tweet with id {TweetId} not found, skipping", tweetId);
                    continue;
                }

                // Check if the relationship already exists
                var existing = await _collectionTweetRepository.GetByCollectionAndTweetIdAsync(collectionId, tweetId);
                if (existing != null)
                {
                    _logger.LogInformation("Tweet {TweetId} is already in collection {CollectionId}, skipping", tweetId, collectionId);
                    continue;
                }

                // Create new CollectionTweet
                maxOrder++;
                var entity = new CollectionTweet
                {
                    CollectionId = collectionId,
                    TweetId = tweetId,
                    DisplayOrder = maxOrder
                };

                var result = await _collectionTweetRepository.AddAsync(entity);
                addedEntities.Add(result);
            }

            await _collectionTweetRepository.SaveChangesAsync();

            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionTweetDto>>(addedEntities),
                $"Successfully added {addedEntities.Count} tweets to collection");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while assigning tweets to collection {CollectionId}", collectionId);
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.FailureResult(
                $"Error occurred while assigning tweets to collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionTweetDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _collectionTweetRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<CollectionTweetDto>.FailureResult($"CollectionTweet with id {id} not found");
            }

            return ServiceResult<CollectionTweetDto>.SuccessResult(_mapper.Map<CollectionTweetDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting CollectionTweet with id {Id}", id);
            return ServiceResult<CollectionTweetDto>.FailureResult($"Error occurred while getting CollectionTweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> GetByCollectionIdAsync(Guid collectionId)
    {
        try
        {
            var entities = await _collectionTweetRepository.GetByCollectionIdAsync(collectionId);
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionTweetDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting CollectionTweets for collection {CollectionId}", collectionId);
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.FailureResult(
                $"Error occurred while getting CollectionTweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> GetByCollectionIdOrderedByDisplayOrderAsync(Guid collectionId)
    {
        try
        {
            var entities = await _collectionTweetRepository.GetByCollectionIdOrderedByDisplayOrderAsync(collectionId);
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionTweetDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting ordered CollectionTweets for collection {CollectionId}", collectionId);
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.FailureResult(
                $"Error occurred while getting ordered CollectionTweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> GetByTweetIdAsync(Guid tweetId)
    {
        try
        {
            var entities = await _collectionTweetRepository.GetByTweetIdAsync(tweetId);
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionTweetDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting CollectionTweets for tweet {TweetId}", tweetId);
            return ServiceResult<IReadOnlyList<CollectionTweetDto>>.FailureResult(
                $"Error occurred while getting CollectionTweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<(IReadOnlyList<CollectionTweetDto> Items, int TotalCount)>> GetPagedAsync(int pageIndex, int pageSize)
    {
        try
        {
            var result = await _collectionTweetRepository.GetPagedAsync(pageIndex, pageSize);
            var dtos = _mapper.Map<IReadOnlyList<CollectionTweetDto>>(result.Items);

            return ServiceResult<(IReadOnlyList<CollectionTweetDto> Items, int TotalCount)>.SuccessResult(
                (dtos, result.TotalCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting paged CollectionTweets");
            return ServiceResult<(IReadOnlyList<CollectionTweetDto> Items, int TotalCount)>.FailureResult(
                $"Error occurred while getting paged CollectionTweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> RemoveTweetFromCollectionAsync(Guid collectionId, Guid tweetId)
    {
        try
        {
            var entity = await _collectionTweetRepository.GetByCollectionAndTweetIdAsync(collectionId, tweetId);
            if (entity == null)
            {
                return ServiceResult<bool>.FailureResult($"Tweet with id {tweetId} not found in collection with id {collectionId}");
            }

            await _collectionTweetRepository.DeleteAsync(entity);
            await _collectionTweetRepository.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true, "Tweet removed from collection successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing tweet {TweetId} from collection {CollectionId}",
                tweetId, collectionId);
            return ServiceResult<bool>.FailureResult($"Error occurred while removing tweet from collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionTweetDto>> UpdateAsync(Guid id, CollectionTweetDto entityDto)
    {
        try
        {
            var existingEntity = await _collectionTweetRepository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return ServiceResult<CollectionTweetDto>.FailureResult($"CollectionTweet with id {id} not found");
            }

            _mapper.Map(entityDto, existingEntity);
            await _collectionTweetRepository.UpdateAsync(existingEntity);
            await _collectionTweetRepository.SaveChangesAsync();

            return ServiceResult<CollectionTweetDto>.SuccessResult(_mapper.Map<CollectionTweetDto>(existingEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating CollectionTweet with id {Id}", id);
            return ServiceResult<CollectionTweetDto>.FailureResult($"Error occurred while updating CollectionTweet: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> UpdateTweetDisplayOrderInCollectionAsync(Guid collectionId, Dictionary<Guid, int> tweetOrderUpdates)
    {
        try
        {
            // Verify collection exists
            var collection = await _collectionRepository.GetByIdAsync(collectionId);
            if (collection == null)
            {
                return ServiceResult<bool>.FailureResult($"Collection with id {collectionId} not found");
            }

            foreach (var update in tweetOrderUpdates)
            {
                var tweetId = update.Key;
                var newOrder = update.Value;

                // Get the collection-tweet relationship
                var collectionTweet = await _collectionTweetRepository.GetByCollectionAndTweetIdAsync(collectionId, tweetId);
                if (collectionTweet == null)
                {
                    _logger.LogWarning("Tweet {TweetId} not found in collection {CollectionId}, skipping", tweetId, collectionId);
                    continue;
                }

                // Update display order
                collectionTweet.DisplayOrder = newOrder;
                await _collectionTweetRepository.UpdateAsync(collectionTweet);
            }

            await _collectionTweetRepository.SaveChangesAsync();
            return ServiceResult<bool>.SuccessResult(true, "Display orders updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating tweet display orders in collection {CollectionId}", collectionId);
            return ServiceResult<bool>.FailureResult($"Error occurred while updating tweet display orders: {ex.Message}");
        }
    }
}