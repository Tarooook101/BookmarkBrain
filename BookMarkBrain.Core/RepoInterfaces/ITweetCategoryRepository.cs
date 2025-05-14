using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.RepoInterfaces;

public interface ITweetCategoryRepository : IRepository<TweetCategory>
{
    Task<IReadOnlyList<TweetCategory>> GetByTweetIdAsync(Guid tweetId);

    Task<IReadOnlyList<TweetCategory>> GetByCategoryIdAsync(Guid categoryId);

    Task<TweetCategory> GetByTweetAndCategoryIdAsync(Guid tweetId, Guid categoryId);

    Task<IReadOnlyList<TweetCategory>> GetAllWithDetailsAsync();

    Task<(IReadOnlyList<TweetCategory> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize);
}