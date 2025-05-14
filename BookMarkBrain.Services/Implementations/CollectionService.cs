

using BookMarkBrain.Core.DTOs.Collection;
using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using FluentValidation;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Implementations;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly ICollectionTweetRepository _collectionTweetRepository;
    private readonly IValidator<CreateCollectionDto> _createValidator;
    private readonly IValidator<UpdateCollectionDto> _updateValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<CollectionService> _logger;

    public CollectionService(
        ICollectionRepository collectionRepository,
        ICollectionTweetRepository collectionTweetRepository,
        IValidator<CreateCollectionDto> createValidator,
        IValidator<UpdateCollectionDto> updateValidator,
        IMapper mapper,
        ILogger<CollectionService> logger)
    {
        _collectionRepository = collectionRepository ?? throw new ArgumentNullException(nameof(collectionRepository));
        _collectionTweetRepository = collectionTweetRepository ?? throw new ArgumentNullException(nameof(collectionTweetRepository));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<CollectionDto>> CreateAsync(CollectionDto entityDto)
    {
        try
        {
            var entity = _mapper.Map<Collection>(entityDto);
            var result = await _collectionRepository.AddAsync(entity);
            await _collectionRepository.SaveChangesAsync();

            return ServiceResult<CollectionDto>.SuccessResult(_mapper.Map<CollectionDto>(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating Collection");
            return ServiceResult<CollectionDto>.FailureResult($"Error occurred while creating Collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionDto>> CreateCollectionAsync(CreateCollectionDto createCollectionDto)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(createCollectionDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<CollectionDto>.FailureResult(
                    "Validation failed",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var collection = _mapper.Map<Collection>(createCollectionDto);
            var result = await _collectionRepository.AddAsync(collection);
            await _collectionRepository.SaveChangesAsync();

            return ServiceResult<CollectionDto>.SuccessResult(_mapper.Map<CollectionDto>(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating Collection");
            return ServiceResult<CollectionDto>.FailureResult($"Error occurred while creating Collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _collectionRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<bool>.FailureResult($"Collection with id {id} not found");
            }

            await _collectionRepository.DeleteAsync(entity);
            await _collectionRepository.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting Collection with id {Id}", id);
            return ServiceResult<bool>.FailureResult($"Error occurred while deleting Collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _collectionRepository.GetAllAsync();
            return ServiceResult<IReadOnlyList<CollectionDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionDto>>(entities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all Collections");
            return ServiceResult<IReadOnlyList<CollectionDto>>.FailureResult($"Error occurred while getting Collections: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _collectionRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return ServiceResult<CollectionDto>.FailureResult($"Collection with id {id} not found");
            }

            return ServiceResult<CollectionDto>.SuccessResult(_mapper.Map<CollectionDto>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting Collection with id {Id}", id);
            return ServiceResult<CollectionDto>.FailureResult($"Error occurred while getting Collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionDto>>> GetCollectionsOrderedByDisplayOrderAsync()
    {
        try
        {
            var collections = await _collectionRepository.GetCollectionsOrderedByDisplayOrderAsync();
            return ServiceResult<IReadOnlyList<CollectionDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionDto>>(collections));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting ordered collections");
            return ServiceResult<IReadOnlyList<CollectionDto>>.FailureResult(
                $"Error occurred while getting ordered collections: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionDetailDto>> GetCollectionWithTweetsAsync(Guid id)
    {
        try
        {
            var collection = await _collectionRepository.GetCollectionWithTweetsAsync(id);
            if (collection == null)
            {
                return ServiceResult<CollectionDetailDto>.FailureResult($"Collection with id {id} not found");
            }

            return ServiceResult<CollectionDetailDto>.SuccessResult(
                _mapper.Map<CollectionDetailDto>(collection));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting collection with tweets for id {Id}", id);
            return ServiceResult<CollectionDetailDto>.FailureResult(
                $"Error occurred while getting collection with tweets: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionDto>>> GetPublicCollectionsAsync()
    {
        try
        {
            var collections = await _collectionRepository.GetPublicCollectionsAsync();
            return ServiceResult<IReadOnlyList<CollectionDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionDto>>(collections));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting public collections");
            return ServiceResult<IReadOnlyList<CollectionDto>>.FailureResult(
                $"Error occurred while getting public collections: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CollectionDto>>> SearchCollectionsAsync(string searchTerm)
    {
        try
        {
            var collections = await _collectionRepository.SearchCollectionsAsync(searchTerm);
            return ServiceResult<IReadOnlyList<CollectionDto>>.SuccessResult(
                _mapper.Map<IReadOnlyList<CollectionDto>>(collections));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching collections with term: {SearchTerm}", searchTerm);
            return ServiceResult<IReadOnlyList<CollectionDto>>.FailureResult(
                $"Error occurred while searching collections: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionDto>> UpdateAsync(Guid id, CollectionDto entityDto)
    {
        try
        {
            var existingEntity = await _collectionRepository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return ServiceResult<CollectionDto>.FailureResult($"Collection with id {id} not found");
            }

            _mapper.Map(entityDto, existingEntity);
            await _collectionRepository.UpdateAsync(existingEntity);
            await _collectionRepository.SaveChangesAsync();

            return ServiceResult<CollectionDto>.SuccessResult(_mapper.Map<CollectionDto>(existingEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating Collection with id {Id}", id);
            return ServiceResult<CollectionDto>.FailureResult($"Error occurred while updating Collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<CollectionDto>> UpdateCollectionAsync(Guid id, UpdateCollectionDto updateCollectionDto)
    {
        try
        {
            var validationResult = await _updateValidator.ValidateAsync(updateCollectionDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<CollectionDto>.FailureResult(
                    "Validation failed",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var existingCollection = await _collectionRepository.GetByIdAsync(id);
            if (existingCollection == null)
            {
                return ServiceResult<CollectionDto>.FailureResult($"Collection with id {id} not found");
            }

            _mapper.Map(updateCollectionDto, existingCollection);
            await _collectionRepository.UpdateAsync(existingCollection);
            await _collectionRepository.SaveChangesAsync();

            return ServiceResult<CollectionDto>.SuccessResult(_mapper.Map<CollectionDto>(existingCollection));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating Collection with id {Id}", id);
            return ServiceResult<CollectionDto>.FailureResult($"Error occurred while updating Collection: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> UpdateCollectionDisplayOrderAsync(Dictionary<Guid, int> collectionOrderUpdates)
    {
        try
        {
            foreach (var update in collectionOrderUpdates)
            {
                var collection = await _collectionRepository.GetByIdAsync(update.Key);
                if (collection == null)
                {
                    return ServiceResult<bool>.FailureResult($"Collection with id {update.Key} not found");
                }

                collection.DisplayOrder = update.Value;
                await _collectionRepository.UpdateAsync(collection);
            }

            await _collectionRepository.SaveChangesAsync();
            return ServiceResult<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating collection display orders");
            return ServiceResult<bool>.FailureResult($"Error occurred while updating collection display orders: {ex.Message}");
        }
    }
}