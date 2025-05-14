namespace BookMarkBrain.Core.Entities;

public class CollectionTweet : BaseEntity
{
    public Guid CollectionId { get; set; }
    public Guid TweetId { get; set; }
    public int DisplayOrder { get; set; }

    public virtual Collection Collection { get; set; }
    public virtual Tweet Tweet { get; set; }
}
