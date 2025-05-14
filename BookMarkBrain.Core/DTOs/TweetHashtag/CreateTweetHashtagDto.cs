

namespace BookMarkBrain.Core.DTOs.TweetHashtag;

/// <summary>
/// DTO for creating a new TweetHashtag relationship
/// </summary>
public class CreateTweetHashtagDto
{
    public Guid TweetId { get; set; }
    public Guid HashtagId { get; set; }
}
