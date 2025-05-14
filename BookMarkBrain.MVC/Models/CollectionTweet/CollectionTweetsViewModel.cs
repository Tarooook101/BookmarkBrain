namespace BookMarkBrain.MVC.Models.CollectionTweet;

public class CollectionTweetsViewModel
{
    public Guid CollectionId { get; set; }
    public string CollectionName { get; set; }
    public string CollectionDescription { get; set; }
    public List<Tweet.TweetViewModel> Tweets { get; set; } = new List<Tweet.TweetViewModel>();
}
