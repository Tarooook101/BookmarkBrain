namespace BookMarkBrain.MVC.Models.CollectionTweet;

public class CollectionTweetDetailViewModel : CollectionTweetViewModel
{
    public Collection.CollectionViewModel Collection { get; set; }
    public Tweet.TweetViewModel Tweet { get; set; }
}