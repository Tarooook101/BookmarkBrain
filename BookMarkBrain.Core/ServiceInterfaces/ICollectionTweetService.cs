using BookMarkBrain.Core.DTOs.CollectionTweet;
using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.ServiceInterfaces;

public interface ICollectionTweetService : IBaseService<CollectionTweet, CollectionTweetDto>
{
    Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> GetByCollectionIdAsync(Guid collectionId);
    Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> GetByTweetIdAsync(Guid tweetId);
    Task<ServiceResult<IReadOnlyList<CollectionTweetDetailDto>>> GetAllWithDetailsAsync();
    Task<ServiceResult<CollectionTweetDto>> CreateCollectionTweetAsync(CreateCollectionTweetDto createDto);
    Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> GetByCollectionIdOrderedByDisplayOrderAsync(Guid collectionId);
    Task<ServiceResult<(IReadOnlyList<CollectionTweetDto> Items, int TotalCount)>> GetPagedAsync(int pageIndex, int pageSize);
    Task<ServiceResult<IReadOnlyList<CollectionTweetDto>>> AssignTweetsToCollectionAsync(Guid collectionId, List<Guid> tweetIds);
    Task<ServiceResult<bool>> RemoveTweetFromCollectionAsync(Guid collectionId, Guid tweetId);
    Task<ServiceResult<bool>> UpdateTweetDisplayOrderInCollectionAsync(Guid collectionId, Dictionary<Guid, int> tweetOrderUpdates);
}