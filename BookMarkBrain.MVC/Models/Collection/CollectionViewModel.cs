using BookMarkBrain.MVC.Models.Common;

namespace BookMarkBrain.MVC.Models.Collection;

public class CollectionViewModel : BaseViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }

    // Optional properties for display purposes
    public int TweetCount { get; set; }
}