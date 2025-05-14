

namespace BookMarkBrain.Core.DTOs.CollectionTweet;

public class CollectionTweetDto
{
    public Guid Id { get; set; }
    public Guid CollectionId { get; set; }
    public Guid TweetId { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}