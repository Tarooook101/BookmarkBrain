using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.RepoInterfaces;

public interface ICollectionRepository : IRepository<Collection>
{
    Task<IReadOnlyList<Collection>> GetCollectionsOrderedByDisplayOrderAsync();
    Task<Collection> GetCollectionWithTweetsAsync(Guid id);
    Task<IReadOnlyList<Collection>> GetPublicCollectionsAsync();
    Task<IReadOnlyList<Collection>> SearchCollectionsAsync(string searchTerm);
}