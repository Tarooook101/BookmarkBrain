
namespace BookMarkBrain.Core.DTOs.Tweet;

public class TweetDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string AuthorUsername { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime? TweetDate { get; set; }
    public string ImageUrl { get; set; }
    public bool IsSeen { get; set; }
    public string PlatformName { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
