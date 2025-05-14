

namespace BookMarkBrain.Core.DTOs.TweetHashtag;

/// <summary>
/// Expanded DTO that includes detailed Tweet and Hashtag information
/// </summary>
public class TweetHashtagWithDetailsDto
{
    public Guid Id { get; set; }
    public Guid TweetId { get; set; }
    public Guid HashtagId { get; set; }

    // Tweet details
    public string TweetContent { get; set; }
    public string AuthorUsername { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime? TweetDate { get; set; }

    // Hashtag details
    public string HashtagName { get; set; }
    public string HashtagDescription { get; set; }
    public bool IsPopular { get; set; }
    public int UsageCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}