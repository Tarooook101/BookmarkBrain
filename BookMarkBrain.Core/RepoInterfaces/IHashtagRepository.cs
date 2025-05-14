using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.RepoInterfaces;

public interface IHashtagRepository : IRepository<Hashtag>
{
    Task<Hashtag> GetByNameAsync(string name);
    Task<IReadOnlyList<Hashtag>> GetPopularHashtagsAsync();
    Task<int> IncrementUsageCountAsync(Guid id);
}
