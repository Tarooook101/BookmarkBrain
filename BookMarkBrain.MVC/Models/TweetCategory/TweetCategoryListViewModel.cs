namespace BookMarkBrain.MVC.Models.TweetCategory;

public class TweetCategoryListViewModel
{
    public List<TweetCategoryViewModel> TweetCategories { get; set; } = new List<TweetCategoryViewModel>();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; } = 10;
    public string SearchTerm { get; set; }
    public Guid? FilterByCategoryId { get; set; }
    public Guid? FilterByTweetId { get; set; }
}