using BookMarkBrain.MVC.Models.Common;

namespace BookMarkBrain.MVC.Models.TweetCategory;

public class TweetCategoryViewModel : BaseViewModel
{
    public Guid TweetId { get; set; }
    public string TweetContent { get; set; }
    public string AuthorUsername { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryColorHex { get; set; }
    public bool IsSeen { get; set; }
    public string PlatformName { get; set; }
    public string OriginalUrl { get; set; }
}