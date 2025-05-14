using BookMarkBrain.Core.DTOs.TweetHashtag;
using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.ServiceInterfaces;

public interface ITweetHashtagService : IBaseService<TweetHashtag, TweetHashtagDto>
{
    Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByTweetIdAsync(Guid tweetId);
    Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByHashtagIdAsync(Guid hashtagId);
    Task<ServiceResult<TweetHashtagDto>> CreateTweetHashtagAsync(CreateTweetHashtagDto createDto);
    Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetAllWithDetailsAsync();
    Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByTweetIdWithDetailsAsync(Guid tweetId);
    Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByHashtagIdWithDetailsAsync(Guid hashtagId);
    Task<ServiceResult<bool>> RemoveTweetHashtagAsync(Guid tweetId, Guid hashtagId);
}