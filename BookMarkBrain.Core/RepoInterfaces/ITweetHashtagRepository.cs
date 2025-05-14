using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.RepoInterfaces;

public interface ITweetHashtagRepository : IRepository<TweetHashtag>
{
    Task<IReadOnlyList<TweetHashtag>> GetByTweetIdAsync(Guid tweetId);

    Task<IReadOnlyList<TweetHashtag>> GetByHashtagIdAsync(Guid hashtagId);

    Task<TweetHashtag> GetByTweetAndHashtagIdAsync(Guid tweetId, Guid hashtagId);

    Task<IReadOnlyList<TweetHashtag>> GetAllWithDetailsAsync();

    Task<IReadOnlyList<TweetHashtag>> GetByTweetIdWithDetailsAsync(Guid tweetId);
    Task<IReadOnlyList<TweetHashtag>> GetByHashtagIdWithDetailsAsync(Guid hashtagId);
}