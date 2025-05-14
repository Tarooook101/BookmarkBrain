using BookMarkBrain.Core.DTOs.TweetHashtag;
using BookMarkBrain.Core.ServiceInterfaces;

namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;

public interface ITweetHashtagApiService
{
    Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetAllTweetHashtagsAsync();
    Task<ServiceResult<TweetHashtagDto>> GetTweetHashtagByIdAsync(Guid id);
    Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByTweetIdAsync(Guid tweetId);
    Task<ServiceResult<IReadOnlyList<TweetHashtagDto>>> GetByHashtagIdAsync(Guid hashtagId);
    Task<ServiceResult<TweetHashtagDto>> CreateTweetHashtagAsync(CreateTweetHashtagDto createTweetHashtagDto);
    Task<ServiceResult<bool>> DeleteTweetHashtagAsync(Guid id);
    Task<ServiceResult<bool>> RemoveTweetHashtagAsync(Guid tweetId, Guid hashtagId);

    // Methods for detailed relationships
    Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetAllWithDetailsAsync();
    Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByTweetIdWithDetailsAsync(Guid tweetId);
    Task<ServiceResult<IReadOnlyList<TweetHashtagWithDetailsDto>>> GetByHashtagIdWithDetailsAsync(Guid hashtagId);
}