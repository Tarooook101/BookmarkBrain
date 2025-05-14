namespace BookMarkBrain.MVC.Models.Tweet;

public class UpdateTweetViewModel
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
    public List<string> HashtagNames { get; set; } = new List<string>(); // Hashtags to associate
}
