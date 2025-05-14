using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Core.ServiceInterfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace BookMarkBrain.Services.Implementations;

public class BaseService<T, TDto> : IBaseService<T, TDto>
    where T : BaseEntity
    where TDto : class
{
    protected readonly IRepository<T> _repository;
    protected readonly ILogger<BaseService<T, TDto>> _logger;

    public BaseService(IRepository<T> repository, ILogger<BaseService<T, TDto>> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<ServiceResult<TDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return ServiceResult<TDto>.FailureResult($"Entity with ID {id} not found");

            var result = entity.Adapt<TDto>();
            return ServiceResult<TDto>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity with ID {Id}", id);
            return ServiceResult<TDto>.FailureResult($"Error getting entity: {ex.Message}");
        }
    }

    public virtual async Task<ServiceResult<IReadOnlyList<TDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var result = entities.Adapt<IReadOnlyList<TDto>>();
            return ServiceResult<IReadOnlyList<TDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities");
            return ServiceResult<IReadOnlyList<TDto>>.FailureResult($"Error getting entities: {ex.Message}");
        }
    }

    public virtual async Task<ServiceResult<TDto>> CreateAsync(TDto entityDto)
    {
        try
        {
            var entity = entityDto.Adapt<T>();
            var createdEntity = await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var result = createdEntity.Adapt<TDto>();
            return ServiceResult<TDto>.SuccessResult(result, "Entity created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            return ServiceResult<TDto>.FailureResult($"Error creating entity: {ex.Message}");
        }
    }

    public virtual async Task<ServiceResult<TDto>> UpdateAsync(Guid id, TDto entityDto)
    {
        try
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                return ServiceResult<TDto>.FailureResult($"Entity with ID {id} not found");

            // Map properties from DTO to existing entity
            entityDto.Adapt(existingEntity);

            await _repository.UpdateAsync(existingEntity);
            await _repository.SaveChangesAsync();

            var result = existingEntity.Adapt<TDto>();
            return ServiceResult<TDto>.SuccessResult(result, "Entity updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity with ID {Id}", id);
            return ServiceResult<TDto>.FailureResult($"Error updating entity: {ex.Message}");
        }
    }

    public virtual async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return ServiceResult<bool>.FailureResult($"Entity with ID {id} not found");

            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true, "Entity deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity with ID {Id}", id);
            return ServiceResult<bool>.FailureResult($"Error deleting entity: {ex.Message}");
        }
    }
}
