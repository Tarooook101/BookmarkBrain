using BookMarkBrain.MVC.Models.TweetCategory;

namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;

public interface ITweetCategoryApiService
{
    Task<TweetCategoryViewModel> GetByIdAsync(Guid id);
    Task<List<TweetCategoryViewModel>> GetAllAsync();
    Task<List<TweetCategoryViewModel>> GetByTweetIdAsync(Guid tweetId);
    Task<List<TweetCategoryViewModel>> GetByCategoryIdAsync(Guid categoryId);
    Task<(List<TweetCategoryViewModel> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize);
    Task<TweetCategoryViewModel> CreateAsync(TweetCategoryCreateViewModel viewModel);
    Task<TweetCategoryViewModel> UpdateAsync(TweetCategoryUpdateViewModel viewModel);
    Task<bool> DeleteAsync(Guid id);
    Task<List<TweetCategoryViewModel>> AssignCategoriesToTweetAsync(Guid tweetId, List<Guid> categoryIds);
    Task<bool> RemoveCategoryFromTweetAsync(Guid tweetId, Guid categoryId);
}