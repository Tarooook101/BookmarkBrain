using BookMarkBrain.MVC.Models.Common;

namespace BookMarkBrain.MVC.Models.Tweet;

public class TweetListViewModel
{
    public List<TweetViewModel> Tweets { get; set; } = new List<TweetViewModel>();
    public TweetSearchViewModel SearchCriteria { get; set; } = new TweetSearchViewModel();
    public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
}
