using BookMarkBrain.MVC.Models.Common;

namespace BookMarkBrain.MVC.Models.TweetHashtag;

public class TweetHashtagViewModel : BaseViewModel
{
    public Guid TweetId { get; set; }
    public Guid HashtagId { get; set; }

    // Display properties
    public string TweetContent { get; set; }
    public string HashtagName { get; set; }
}