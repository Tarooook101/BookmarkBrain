using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.ServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;

public interface IHashtagApiService
{
    Task<ServiceResult<IReadOnlyList<HashtagDto>>> GetAllHashtagsAsync();
    Task<ServiceResult<HashtagDto>> GetHashtagByIdAsync(Guid id);
    Task<ServiceResult<HashtagDto>> GetHashtagByNameAsync(string name);
    Task<ServiceResult<IReadOnlyList<HashtagDto>>> GetPopularHashtagsAsync();
    Task<ServiceResult<HashtagDto>> CreateHashtagAsync(CreateHashtagDto createHashtagDto);
    Task<ServiceResult<HashtagDto>> UpdateHashtagAsync(Guid id, UpdateHashtagDto updateHashtagDto);
    Task<ServiceResult<bool>> DeleteHashtagAsync(Guid id);
    Task<ServiceResult<int>> IncrementUsageCountAsync(Guid id);
}
