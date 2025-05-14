using System.ComponentModel.DataAnnotations;

namespace BookMarkBrain.MVC.Models.TweetHashtag;

public class CreateTweetHashtagViewModel
{
    [Required(ErrorMessage = "Tweet is required")]
    public Guid TweetId { get; set; }

    [Required(ErrorMessage = "Hashtag is required")]
    public Guid HashtagId { get; set; }

    // Optional properties for display/selection purposes
    public List<TweetSelectListItem> AvailableTweets { get; set; } = new List<TweetSelectListItem>();
    public List<HashtagSelectListItem> AvailableHashtags { get; set; } = new List<HashtagSelectListItem>();
}