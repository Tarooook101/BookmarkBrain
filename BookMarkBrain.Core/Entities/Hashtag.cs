

namespace BookMarkBrain.Core.Entities;

public class Hashtag : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsPopular { get; set; }
    public int UsageCount { get; set; }

    public virtual ICollection<TweetHashtag> TweetHashtags { get; set; } = new List<TweetHashtag>();

}
