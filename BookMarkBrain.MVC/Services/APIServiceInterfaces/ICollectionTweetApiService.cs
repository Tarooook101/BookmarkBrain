using BookMarkBrain.MVC.Models.CollectionTweet;

namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;

public interface ICollectionTweetApiService
{
    Task<List<CollectionTweetViewModel>> GetAllCollectionTweetsAsync();
    Task<List<CollectionTweetDetailViewModel>> GetAllCollectionTweetsWithDetailsAsync();
    Task<CollectionTweetViewModel> GetCollectionTweetByIdAsync(Guid id);
    Task<List<CollectionTweetViewModel>> GetCollectionTweetsByCollectionIdAsync(Guid collectionId);
    Task<List<CollectionTweetViewModel>> GetCollectionTweetsByCollectionIdOrderedAsync(Guid collectionId);
    Task<List<CollectionTweetViewModel>> GetCollectionTweetsByTweetIdAsync(Guid tweetId);
    Task<(List<CollectionTweetViewModel> Items, int TotalCount)> GetPagedCollectionTweetsAsync(int pageIndex, int pageSize);
    Task<CollectionTweetViewModel> CreateCollectionTweetAsync(CreateCollectionTweetViewModel createViewModel);
    Task<List<CollectionTweetViewModel>> AssignTweetsToCollectionAsync(Guid collectionId, List<Guid> tweetIds);
    Task<bool> RemoveTweetFromCollectionAsync(Guid collectionId, Guid tweetId);
    Task<bool> UpdateTweetDisplayOrderInCollectionAsync(Guid collectionId, Dictionary<Guid, int> tweetOrderUpdates);
}
