namespace BookMarkBrain.MVC.Models.Collection;

public class CollectionListViewModel
{
    public List<CollectionViewModel> Collections { get; set; } = new List<CollectionViewModel>();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public string SearchTerm { get; set; }
}