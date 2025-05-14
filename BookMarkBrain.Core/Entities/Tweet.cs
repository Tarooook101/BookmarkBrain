

namespace BookMarkBrain.Core.Entities;


public class Tweet : BaseEntity
{
    public string Content { get; set; }
    public string AuthorUsername { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime? TweetDate { get; set; }
    public string ImageUrl { get; set; }
    public bool IsSeen { get; set; }
    public string PlatformName { get; set; }
    public Guid? CategoryId { get; set; }

    public virtual ICollection<TweetHashtag> TweetHashtags { get; set; } = new List<TweetHashtag>();
    public virtual ICollection<TweetCategory> TweetCategories { get; set; } = new List<TweetCategory>();

}