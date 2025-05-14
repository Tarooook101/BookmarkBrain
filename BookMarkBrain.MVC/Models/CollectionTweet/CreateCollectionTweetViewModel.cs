namespace BookMarkBrain.MVC.Models.CollectionTweet;

public class CreateCollectionTweetViewModel
{
    public Guid CollectionId { get; set; }
    public Guid TweetId { get; set; }
    public int DisplayOrder { get; set; }
}