namespace BookMarkBrain.MVC.Models.CollectionTweet;

public class TweetAssignmentViewModel
{
    public Guid CollectionId { get; set; }
    public string CollectionName { get; set; }
    public List<TweetSelectionViewModel> AvailableTweets { get; set; } = new List<TweetSelectionViewModel>();
    public List<Guid> SelectedTweetIds { get; set; } = new List<Guid>();
}