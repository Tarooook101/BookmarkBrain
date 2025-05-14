namespace BookMarkBrain.MVC.Models.Tweet;

public class TweetSearchViewModel
{
    public string SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public string HashtagName { get; set; }
    public string PlatformName { get; set; }
    public bool? SeenStatus { get; set; }
}
