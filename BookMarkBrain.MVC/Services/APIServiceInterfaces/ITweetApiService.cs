using BookMarkBrain.MVC.Models.Tweet;

namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;

public interface ITweetApiService
{
    Task<List<TweetViewModel>> GetAllTweetsAsync();
    Task<TweetViewModel> GetTweetByIdAsync(Guid id);
    Task<List<TweetViewModel>> GetTweetsByCategoryAsync(Guid categoryId);
    Task<List<TweetViewModel>> GetTweetsByPlatformAsync(string platformName);
    Task<List<TweetViewModel>> SearchTweetsAsync(string searchTerm);
    Task<TweetViewModel> CreateTweetAsync(CreateTweetViewModel tweetViewModel);
    Task<TweetViewModel> ExtractTweetFromUrlAsync(string url);
    Task<TweetViewModel> UpdateTweetAsync(UpdateTweetViewModel tweetViewModel);
    Task<TweetViewModel> ToggleTweetSeenStatusAsync(Guid id);
    Task<bool> DeleteTweetAsync(Guid id);
}
