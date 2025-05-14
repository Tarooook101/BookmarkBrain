namespace BookMarkBrain.Core.Entities;

public class TweetCategory : BaseEntity
{
    public Guid TweetId { get; set; }
    public Guid CategoryId { get; set; }

    public virtual Tweet Tweet { get; set; }
    public virtual Category Category { get; set; }
}
