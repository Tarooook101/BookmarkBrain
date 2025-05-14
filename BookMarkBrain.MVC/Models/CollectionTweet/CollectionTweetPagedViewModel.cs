namespace BookMarkBrain.MVC.Models.CollectionTweet;

public class CollectionTweetPagedViewModel
{
    public List<CollectionTweetDetailViewModel> CollectionTweets { get; set; } = new List<CollectionTweetDetailViewModel>();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}