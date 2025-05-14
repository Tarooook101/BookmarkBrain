namespace BookMarkBrain.Core.DTOs.TweetCategory;

public class UpdateTweetCategoryDto
{
    public Guid TweetId { get; set; }
    public Guid CategoryId { get; set; }
}