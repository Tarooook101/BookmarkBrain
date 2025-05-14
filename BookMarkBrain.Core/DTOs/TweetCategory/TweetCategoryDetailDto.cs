namespace BookMarkBrain.Core.DTOs.TweetCategory;

public class TweetCategoryDetailDto
{
    public Guid Id { get; set; }
    public Guid TweetId { get; set; }
    public string TweetContent { get; set; }
    public string AuthorUsername { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryColorHex { get; set; }
    public DateTime CreatedAt { get; set; }
}