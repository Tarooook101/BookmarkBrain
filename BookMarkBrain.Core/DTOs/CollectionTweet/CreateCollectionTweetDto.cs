namespace BookMarkBrain.Core.DTOs.CollectionTweet;

public class CreateCollectionTweetDto
{
    public Guid CollectionId { get; set; }
    public Guid TweetId { get; set; }
    public int DisplayOrder { get; set; }
}