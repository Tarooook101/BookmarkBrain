
namespace BookMarkBrain.Core.DTOs.TweetHashtag;


/// <summary>
/// DTO for retrieving TweetHashtag information including relevant Tweet and Hashtag data
/// </summary>
public class TweetHashtagDto
{
    public Guid Id { get; set; }
    public Guid TweetId { get; set; }
    public Guid HashtagId { get; set; }
    public string TweetContent { get; set; }  // Optional: Include for display convenience
    public string HashtagName { get; set; }   // Optional: Include for display convenience
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
