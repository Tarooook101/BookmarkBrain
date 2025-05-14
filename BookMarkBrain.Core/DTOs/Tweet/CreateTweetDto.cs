
namespace BookMarkBrain.Core.DTOs.Tweet;

public class CreateTweetDto
{
    public string Content { get; set; }
    public string AuthorUsername { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime? TweetDate { get; set; }
    public string ImageUrl { get; set; }
    public bool IsSeen { get; set; }
    public string PlatformName { get; set; }
    public Guid? CategoryId { get; set; }
}
