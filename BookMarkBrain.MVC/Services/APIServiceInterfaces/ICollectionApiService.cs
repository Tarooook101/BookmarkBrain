using BookMarkBrain.MVC.Models.Collection;

namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;

public interface ICollectionApiService
{
    Task<List<CollectionViewModel>> GetAllCollectionsAsync();
    Task<List<CollectionViewModel>> GetOrderedCollectionsAsync();
    Task<CollectionViewModel> GetCollectionByIdAsync(Guid id);
    Task<CollectionDetailViewModel> GetCollectionWithTweetsAsync(Guid id);
    Task<List<CollectionViewModel>> GetPublicCollectionsAsync();
    Task<List<CollectionViewModel>> SearchCollectionsAsync(string searchTerm);
    Task<CollectionViewModel> CreateCollectionAsync(CreateCollectionViewModel createViewModel);
    Task<CollectionViewModel> UpdateCollectionAsync(Guid id, UpdateCollectionViewModel updateViewModel);
    Task<bool> UpdateCollectionDisplayOrderAsync(Dictionary<Guid, int> orderUpdates);
    Task<bool> DeleteCollectionAsync(Guid id);
}