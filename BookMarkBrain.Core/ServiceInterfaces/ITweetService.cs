using BookMarkBrain.Core.DTOs.Tweet;
using BookMarkBrain.Core.Entities;


namespace BookMarkBrain.Core.ServiceInterfaces;

public interface ITweetService : IBaseService<Tweet, TweetDto>
{
    Task<ServiceResult<IReadOnlyList<TweetDto>>> GetTweetsByCategoryIdAsync(Guid categoryId);
    Task<ServiceResult<IReadOnlyList<TweetDto>>> GetTweetsByPlatformAsync(string platformName);
    Task<ServiceResult<TweetDto>> ToggleSeenStatusAsync(Guid id);
    Task<ServiceResult<TweetDto>> ExtractTweetFromUrlAsync(string url);
    Task<ServiceResult<IReadOnlyList<TweetDto>>> SearchTweetsAsync(string searchTerm);
}
