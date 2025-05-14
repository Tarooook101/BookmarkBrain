using BookMarkBrain.Core.DTOs.Hashtag;
using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.ServiceInterfaces;

public interface IHashtagService : IBaseService<Hashtag, HashtagDto>
{
    Task<ServiceResult<IReadOnlyList<HashtagDto>>> GetPopularHashtagsAsync();
    Task<ServiceResult<HashtagDto>> GetHashtagByNameAsync(string name);
    Task<ServiceResult<HashtagDto>> CreateHashtagAsync(CreateHashtagDto createHashtagDto);
    Task<ServiceResult<HashtagDto>> UpdateHashtagAsync(Guid id, UpdateHashtagDto updateHashtagDto);
    Task<ServiceResult<int>> IncrementUsageCountAsync(Guid id);
}
