using BookMarkBrain.MVC.Models.Common;

namespace BookMarkBrain.MVC.Models.Tweet;

public class TweetViewModel : BaseViewModel
{
    public string Content { get; set; }
    public string AuthorUsername { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime? TweetDate { get; set; }
    public string ImageUrl { get; set; }
    public bool IsSeen { get; set; }
    public string PlatformName { get; set; }
    public Guid? CategoryId { get; set; }
    public string CategoryName { get; set; } // Added for display purposes
    public List<string> Hashtags { get; set; } = new List<string>(); // For displaying associated hashtags
}
