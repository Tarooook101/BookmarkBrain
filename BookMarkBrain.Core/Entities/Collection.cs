namespace BookMarkBrain.Core.Entities;

public class Collection : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }

    public virtual ICollection<CollectionTweet> CollectionTweets { get; set; } = new List<CollectionTweet>();
}
