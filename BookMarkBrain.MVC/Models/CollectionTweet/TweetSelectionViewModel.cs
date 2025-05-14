namespace BookMarkBrain.MVC.Models.CollectionTweet;

public class TweetSelectionViewModel
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string AuthorUsername { get; set; }
    public bool IsSelected { get; set; }
}