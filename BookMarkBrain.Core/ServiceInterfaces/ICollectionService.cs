using BookMarkBrain.Core.DTOs.Collection;
using BookMarkBrain.Core.Entities;


namespace BookMarkBrain.Core.ServiceInterfaces;

public interface ICollectionService : IBaseService<Collection, CollectionDto>
{
    Task<ServiceResult<IReadOnlyList<CollectionDto>>> GetCollectionsOrderedByDisplayOrderAsync();
    Task<ServiceResult<CollectionDetailDto>> GetCollectionWithTweetsAsync(Guid id);
    Task<ServiceResult<IReadOnlyList<CollectionDto>>> GetPublicCollectionsAsync();
    Task<ServiceResult<CollectionDto>> CreateCollectionAsync(CreateCollectionDto createCollectionDto);
    Task<ServiceResult<CollectionDto>> UpdateCollectionAsync(Guid id, UpdateCollectionDto updateCollectionDto);
    Task<ServiceResult<bool>> UpdateCollectionDisplayOrderAsync(Dictionary<Guid, int> collectionOrderUpdates);
    Task<ServiceResult<IReadOnlyList<CollectionDto>>> SearchCollectionsAsync(string searchTerm);
}