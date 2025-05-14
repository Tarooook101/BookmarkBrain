using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.RepoInterfaces;

public interface ITweetRepository : IRepository<Tweet>
{
    Task<IReadOnlyList<Tweet>> GetTweetsByCategoryIdAsync(Guid categoryId);
    Task<IReadOnlyList<Tweet>> GetTweetsByPlatformAsync(string platformName);
    Task<IReadOnlyList<Tweet>> SearchTweetsAsync(string searchTerm);
}
