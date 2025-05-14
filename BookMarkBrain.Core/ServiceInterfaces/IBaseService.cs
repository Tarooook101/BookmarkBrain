

namespace BookMarkBrain.Core.ServiceInterfaces;
public interface IBaseService<T, TDto> where T : class where TDto : class
{
    Task<ServiceResult<TDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<IReadOnlyList<TDto>>> GetAllAsync();
    Task<ServiceResult<TDto>> CreateAsync(TDto entityDto);
    Task<ServiceResult<TDto>> UpdateAsync(Guid id, TDto entityDto);
    Task<ServiceResult<bool>> DeleteAsync(Guid id);
}