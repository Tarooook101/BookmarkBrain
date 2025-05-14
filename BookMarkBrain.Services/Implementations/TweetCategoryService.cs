using BookMarkBrain.Core.DTOs.TweetCategory;
using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Implementations;

public class TweetCategoryService : BaseService<TweetCategory, TweetCategoryDto>, ITweetCategoryService
{
    private readonly ITweetCategoryRepository _tweetCategoryRepository;
    private readonly ITweetRepository _tweetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CreateTweetCategoryDto> _createValidator;
    private readonly IValidator<UpdateTweetCategoryDto> _updateValidator;

    public TweetCategoryService(
        ITweetCategoryRepository tweetCategoryRepository,
        ITweetRepository tweetRepository,
        ICategoryRepository categoryRepository,
        IValidator<CreateTweetCategoryDto> createValidator,
        IValidator<UpdateTweetCategoryDto> updateValidator,
        ILogger<TweetCategoryService> logger)
        : base(tweetCategoryRepository, logger)
    {
        _tweetCategoryRepository = tweetCategoryRepository ?? throw new ArgumentNullException(nameof(tweetCategoryRepository));
        _tweetRepository = tweetRepository ?? throw new ArgumentNullException(nameof(tweetRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
    }

    public async Task<ServiceResult<IReadOnlyList<TweetCategoryDto>>> GetByTweetIdAsync(Guid tweetId)
    {
        try
        {
            var tweetCategories = await _tweetCategoryRepository.GetByTweetIdAsync(tweetId);
            if (tweetCategories == null || !tweetCategories.Any())
                return ServiceResult<IReadOnlyList<TweetCategoryDto>>.SuccessResult(new List<TweetCategoryDto>());

            var tweetCategoryDtos = tweetCategories.Adapt<IReadOnlyList<TweetCategoryDto>>();
            return ServiceResult<IReadOnlyList<TweetCategoryDto>>.SuccessResult(tweetCategoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet categories for tweet {TweetId}", tweetId);
            return ServiceResult<IReadOnlyList<TweetCategoryDto>>.FailureResult($"Error getting tweet categories: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetCategoryDto>>> GetByCategoryIdAsync(Guid categoryId)
    {
        try
        {
            var tweetCategories = await _tweetCategoryRepository.GetByCategoryIdAsync(categoryId);
            if (tweetCategories == null || !tweetCategories.Any())
                return ServiceResult<IReadOnlyList<TweetCategoryDto>>.SuccessResult(new List<TweetCategoryDto>());

            var tweetCategoryDtos = tweetCategories.Adapt<IReadOnlyList<TweetCategoryDto>>();
            return ServiceResult<IReadOnlyList<TweetCategoryDto>>.SuccessResult(tweetCategoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tweet categories for category {CategoryId}", categoryId);
            return ServiceResult<IReadOnlyList<TweetCategoryDto>>.FailureResult($"Error getting tweet categories: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetCategoryDetailDto>>> GetAllWithDetailsAsync()
    {
        try
        {
            var tweetCategories = await _tweetCategoryRepository.GetAllWithDetailsAsync();
            if (tweetCategories == null || !tweetCategories.Any())
                return ServiceResult<IReadOnlyList<TweetCategoryDetailDto>>.SuccessResult(new List<TweetCategoryDetailDto>());

            var tweetCategoryDetailDtos = tweetCategories.Select(tc => new TweetCategoryDetailDto
            {
                Id = tc.Id,
                TweetId = tc.TweetId,
                TweetContent = tc.Tweet?.Content,
                AuthorUsername = tc.Tweet?.AuthorUsername,
                CategoryId = tc.CategoryId,
                CategoryName = tc.Category?.Name,
                CategoryColorHex = tc.Category?.ColorHex,
                CreatedAt = tc.CreatedAt
            }).ToList();

            return ServiceResult<IReadOnlyList<TweetCategoryDetailDto>>.SuccessResult(tweetCategoryDetailDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tweet categories with details");
            return ServiceResult<IReadOnlyList<TweetCategoryDetailDto>>.FailureResult($"Error getting tweet categories with details: {ex.Message}");
        }
    }

    public async Task<ServiceResult<TweetCategoryDto>> CreateTweetCategoryAsync(CreateTweetCategoryDto createDto)
    {
        try
        {
            // Validate the create DTO
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<TweetCategoryDto>.FailureResult(
                    "Tweet category validation failed",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Check if tweet exists
            var tweet = await _tweetRepository.GetByIdAsync(createDto.TweetId);
            if (tweet == null)
                return ServiceResult<TweetCategoryDto>.FailureResult($"Tweet with ID {createDto.TweetId} not found");

            // Check if category exists
            var category = await _categoryRepository.GetByIdAsync(createDto.CategoryId);
            if (category == null)
                return ServiceResult<TweetCategoryDto>.FailureResult($"Category with ID {createDto.CategoryId} not found");

            // Check if the relationship already exists
            var existingRelation = await _tweetCategoryRepository.GetByTweetAndCategoryIdAsync(createDto.TweetId, createDto.CategoryId);
            if (existingRelation != null)
                return ServiceResult<TweetCategoryDto>.FailureResult("This tweet is already assigned to this category");

            // Create the new relationship
            var tweetCategory = createDto.Adapt<TweetCategory>();
            var createdEntity = await _tweetCategoryRepository.AddAsync(tweetCategory);
            await _tweetCategoryRepository.SaveChangesAsync();

            // Return the created entity
            var result = createdEntity.Adapt<TweetCategoryDto>();
            return ServiceResult<TweetCategoryDto>.SuccessResult(result, "Tweet category created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tweet category for Tweet {TweetId} and Category {CategoryId}",
                createDto.TweetId, createDto.CategoryId);
            return ServiceResult<TweetCategoryDto>.FailureResult($"Error creating tweet category: {ex.Message}");
        }
    }

    public async Task<ServiceResult<(IReadOnlyList<TweetCategoryDto> Items, int TotalCount)>> GetPagedAsync(int pageIndex, int pageSize)
    {
        try
        {
            if (pageIndex < 1)
                pageIndex = 1;
            if (pageSize < 1)
                pageSize = 10;

            var (items, totalCount) = await _tweetCategoryRepository.GetPagedAsync(pageIndex, pageSize);
            var dtos = items.Adapt<IReadOnlyList<TweetCategoryDto>>();

            return ServiceResult<(IReadOnlyList<TweetCategoryDto> Items, int TotalCount)>.SuccessResult((dtos, totalCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged tweet categories");
            return ServiceResult<(IReadOnlyList<TweetCategoryDto> Items, int TotalCount)>.FailureResult($"Error getting paged tweet categories: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<TweetCategoryDto>>> AssignCategoriesToTweetAsync(Guid tweetId, List<Guid> categoryIds)
    {
        try
        {
            // Validate tweet exists
            var tweet = await _tweetRepository.GetByIdAsync(tweetId);
            if (tweet == null)
                return ServiceResult<IReadOnlyList<TweetCategoryDto>>.FailureResult($"Tweet with ID {tweetId} not found");

            // Validate categories exist
            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                    return ServiceResult<IReadOnlyList<TweetCategoryDto>>.FailureResult($"Category with ID {categoryId} not found");
            }

            // Get existing relationships
            var existingRelations = await _tweetCategoryRepository.GetByTweetIdAsync(tweetId);
            var existingCategoryIds = existingRelations.Select(tc => tc.CategoryId).ToList();

            // Filter out categories that are already assigned
            var newCategoryIds = categoryIds.Where(id => !existingCategoryIds.Contains(id)).ToList();

            // Create new relationships
            var newRelations = new List<TweetCategory>();
            foreach (var categoryId in newCategoryIds)
            {
                var tweetCategory = new TweetCategory
                {
                    TweetId = tweetId,
                    CategoryId = categoryId
                };
                var createdEntity = await _tweetCategoryRepository.AddAsync(tweetCategory);
                newRelations.Add(createdEntity);
            }

            await _tweetCategoryRepository.SaveChangesAsync();

            // Get all relationships after update
            var allRelations = await _tweetCategoryRepository.GetByTweetIdAsync(tweetId);
            var result = allRelations.Adapt<IReadOnlyList<TweetCategoryDto>>();

            return ServiceResult<IReadOnlyList<TweetCategoryDto>>.SuccessResult(
                result,
                $"Successfully assigned {newCategoryIds.Count} categories to tweet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning categories to tweet {TweetId}", tweetId);
            return ServiceResult<IReadOnlyList<TweetCategoryDto>>.FailureResult($"Error assigning categories: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> RemoveCategoryFromTweetAsync(Guid tweetId, Guid categoryId)
    {
        try
        {
            // Check if the relationship exists
            var relation = await _tweetCategoryRepository.GetByTweetAndCategoryIdAsync(tweetId, categoryId);
            if (relation == null)
                return ServiceResult<bool>.FailureResult("This tweet is not assigned to this category");

            // Delete the relationship
            await _tweetCategoryRepository.DeleteAsync(relation);
            await _tweetCategoryRepository.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true, "Category removed from tweet successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing category {CategoryId} from tweet {TweetId}", categoryId, tweetId);
            return ServiceResult<bool>.FailureResult($"Error removing category: {ex.Message}");
        }
    }
}