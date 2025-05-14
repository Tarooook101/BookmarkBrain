namespace BookMarkBrain.Core.DTOs.TweetCategory;

public class CreateTweetCategoryDto
{
    public Guid TweetId { get; set; }
    public Guid CategoryId { get; set; }
}