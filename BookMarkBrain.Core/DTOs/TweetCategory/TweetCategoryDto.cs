namespace BookMarkBrain.Core.DTOs.TweetCategory;

public class TweetCategoryDto
{
    public Guid Id { get; set; }
    public Guid TweetId { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Optional additional information
    public string TweetContent { get; set; }
    public string CategoryName { get; set; }
}