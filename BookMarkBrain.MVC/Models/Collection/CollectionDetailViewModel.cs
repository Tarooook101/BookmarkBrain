namespace BookMarkBrain.MVC.Models.Collection;

public class CollectionDetailViewModel : CollectionViewModel
{
    public List<Tweet.TweetViewModel> Tweets { get; set; } = new List<Tweet.TweetViewModel>();
}
