using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Implementations;

public class HashtagService : BaseService<Hashtag, HashtagDto>, IHashtagService
{
    private readonly IHashtagRepository _hashtagRepository;
    private readonly IValidator<CreateHashtagDto> _createValidator;
    private readonly IValidator<UpdateHashtagDto> _updateValidator;

    public HashtagService(
        IHashtagRepository hashtagRepository,
        IValidator<CreateHashtagDto> createValidator,
        IValidator<UpdateHashtagDto> updateValidator,
        ILogger<HashtagService> logger)
        : base(hashtagRepository, logger)
    {
        _hashtagRepository = hashtagRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ServiceResult<HashtagDto>> CreateHashtagAsync(CreateHashtagDto createHashtagDto)
    {
        try
        {
            // Validate input
            var validationResult = await _createValidator.ValidateAsync(createHashtagDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<HashtagDto>.FailureResult(
                    "Validation failed for hashtag creation",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Check if hashtag with the same name already exists
            var existingHashtag = await _hashtagRepository.GetByNameAsync(createHashtagDto.Name);
            if (existingHashtag != null)
            {
                return ServiceResult<HashtagDto>.FailureResult($"Hashtag with name '{createHashtagDto.Name}' already exists");
            }

            // Create new hashtag
            var hashtag = createHashtagDto.Adapt<Hashtag>();
            hashtag.UsageCount = 0; // Initialize usage count

            var createdHashtag = await _hashtagRepository.AddAsync(hashtag);
            await _hashtagRepository.SaveChangesAsync();

            _logger.LogInformation("Created new hashtag: {HashtagName} with ID: {HashtagId}", hashtag.Name, hashtag.Id);
            return ServiceResult<HashtagDto>.SuccessResult(createdHashtag.Adapt<HashtagDto>(), "Hashtag created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hashtag with name: {HashtagName}", createHashtagDto.Name);
            return ServiceResult<HashtagDto>.FailureResult($"Error creating hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<HashtagDto>> GetHashtagByNameAsync(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResult<HashtagDto>.FailureResult("Hashtag name cannot be empty");
            }

            var hashtag = await _hashtagRepository.GetByNameAsync(name);
            if (hashtag == null)
            {
                return ServiceResult<HashtagDto>.FailureResult($"Hashtag with name '{name}' not found");
            }

            return ServiceResult<HashtagDto>.SuccessResult(hashtag.Adapt<HashtagDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hashtag by name: {HashtagName}", name);
            return ServiceResult<HashtagDto>.FailureResult($"Error retrieving hashtag: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IReadOnlyList<HashtagDto>>> GetPopularHashtagsAsync()
    {
        try
        {
            var hashtags = await _hashtagRepository.GetPopularHashtagsAsync();
            var hashtagDtos = hashtags.Adapt<IReadOnlyList<HashtagDto>>();
            return ServiceResult<IReadOnlyList<HashtagDto>>.SuccessResult(hashtagDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular hashtags");
            return ServiceResult<IReadOnlyList<HashtagDto>>.FailureResult($"Error retrieving popular hashtags: {ex.Message}");
        }
    }

    public async Task<ServiceResult<int>> IncrementUsageCountAsync(Guid id)
    {
        try
        {
            var usageCount = await _hashtagRepository.IncrementUsageCountAsync(id);
            if (usageCount == 0)
            {
                return ServiceResult<int>.FailureResult($"Hashtag with ID {id} not found");
            }

            _logger.LogInformation("Incremented usage count for hashtag ID: {HashtagId}, new count: {UsageCount}", id, usageCount);
            return ServiceResult<int>.SuccessResult(usageCount, "Usage count incremented successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing usage count for hashtag ID: {HashtagId}", id);
            return ServiceResult<int>.FailureResult($"Error incrementing usage count: {ex.Message}");
        }
    }

    public async Task<ServiceResult<HashtagDto>> UpdateHashtagAsync(Guid id, UpdateHashtagDto updateHashtagDto)
    {
        try
        {
            // Validate input
            var validationResult = await _updateValidator.ValidateAsync(updateHashtagDto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<HashtagDto>.FailureResult(
                    "Validation failed for hashtag update",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Check if hashtag exists
            var existingHashtag = await _hashtagRepository.GetByIdAsync(id);
            if (existingHashtag == null)
            {
                return ServiceResult<HashtagDto>.FailureResult($"Hashtag with ID {id} not found");
            }

            // Check if name is being changed and if new name already exists
            if (existingHashtag.Name != updateHashtagDto.Name)
            {
                var hashtagWithSameName = await _hashtagRepository.GetByNameAsync(updateHashtagDto.Name);
                if (hashtagWithSameName != null && hashtagWithSameName.Id != id)
                {
                    return ServiceResult<HashtagDto>.FailureResult($"Hashtag with name '{updateHashtagDto.Name}' already exists");
                }
            }

            // Update properties
            existingHashtag.Name = updateHashtagDto.Name;
            existingHashtag.Description = updateHashtagDto.Description;
            existingHashtag.IsPopular = updateHashtagDto.IsPopular;

            await _hashtagRepository.UpdateAsync(existingHashtag);
            await _hashtagRepository.SaveChangesAsync();

            _logger.LogInformation("Updated hashtag ID: {HashtagId}, Name: {HashtagName}", id, existingHashtag.Name);
            return ServiceResult<HashtagDto>.SuccessResult(existingHashtag.Adapt<HashtagDto>(), "Hashtag updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hashtag with ID: {HashtagId}", id);
            return ServiceResult<HashtagDto>.FailureResult($"Error updating hashtag: {ex.Message}");
        }
    }
}