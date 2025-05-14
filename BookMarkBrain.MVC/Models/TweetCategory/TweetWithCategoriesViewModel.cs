namespace BookMarkBrain.MVC.Models.TweetCategory;

public class TweetWithCategoriesViewModel
{
    public Guid TweetId { get; set; }
    public string TweetContent { get; set; }
    public string AuthorUsername { get; set; }
    public string OriginalUrl { get; set; }
    public List<CategorySelectItemViewModel> AssignedCategories { get; set; } = new List<CategorySelectItemViewModel>();
    public List<CategorySelectItemViewModel> AvailableCategories { get; set; } = new List<CategorySelectItemViewModel>();
}