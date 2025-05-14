using BookMarkBrain.Core.DTOs.TweetCategory;
using BookMarkBrain.Core.Entities;

namespace BookMarkBrain.Core.ServiceInterfaces;

public interface ITweetCategoryService : IBaseService<TweetCategory, TweetCategoryDto>
{
    Task<ServiceResult<IReadOnlyList<TweetCategoryDto>>> GetByTweetIdAsync(Guid tweetId);

    Task<ServiceResult<IReadOnlyList<TweetCategoryDto>>> GetByCategoryIdAsync(Guid categoryId);

    Task<ServiceResult<IReadOnlyList<TweetCategoryDetailDto>>> GetAllWithDetailsAsync();

    Task<ServiceResult<TweetCategoryDto>> CreateTweetCategoryAsync(CreateTweetCategoryDto createDto);

    Task<ServiceResult<(IReadOnlyList<TweetCategoryDto> Items, int TotalCount)>> GetPagedAsync(int pageIndex, int pageSize);

    Task<ServiceResult<IReadOnlyList<TweetCategoryDto>>> AssignCategoriesToTweetAsync(Guid tweetId, List<Guid> categoryIds);

    Task<ServiceResult<bool>> RemoveCategoryFromTweetAsync(Guid tweetId, Guid categoryId);
}