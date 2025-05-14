using BookMarkBrain.MVC.Models.Common;

namespace BookMarkBrain.MVC.Models.TweetHashtag;

public class TweetHashtagDetailsViewModel : BaseViewModel
{
    public Guid TweetId { get; set; }
    public Guid HashtagId { get; set; }

    // Tweet details
    public string TweetContent { get; set; }
    public string AuthorUsername { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime? TweetDate { get; set; }
    public string ImageUrl { get; set; }
    public bool IsSeen { get; set; }
    public string PlatformName { get; set; }

    // Hashtag details
    public string HashtagName { get; set; }
    public string HashtagDescription { get; set; }
    public bool IsPopular { get; set; }
    public int UsageCount { get; set; }
}