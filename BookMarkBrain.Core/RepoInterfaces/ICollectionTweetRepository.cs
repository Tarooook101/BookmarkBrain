using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.RepoInterfaces;

public interface ICollectionTweetRepository : IRepository<CollectionTweet>
{
    Task<IReadOnlyList<CollectionTweet>> GetByCollectionIdAsync(Guid collectionId);
    Task<IReadOnlyList<CollectionTweet>> GetByTweetIdAsync(Guid tweetId);
    Task<CollectionTweet> GetByCollectionAndTweetIdAsync(Guid collectionId, Guid tweetId);
    Task<IReadOnlyList<CollectionTweet>> GetAllWithDetailsAsync();
    Task<IReadOnlyList<CollectionTweet>> GetByCollectionIdOrderedByDisplayOrderAsync(Guid collectionId);
    Task<(IReadOnlyList<CollectionTweet> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize);
}