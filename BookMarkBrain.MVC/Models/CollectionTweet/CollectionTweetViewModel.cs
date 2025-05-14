using BookMarkBrain.MVC.Models.Common;

namespace BookMarkBrain.MVC.Models.CollectionTweet;

public class CollectionTweetViewModel : BaseViewModel
{
    public Guid CollectionId { get; set; }
    public Guid TweetId { get; set; }
    public int DisplayOrder { get; set; }

    // Optional properties for display purposes
    public string CollectionName { get; set; }
    public string TweetContent { get; set; }
}