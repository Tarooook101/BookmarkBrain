using System.ComponentModel.DataAnnotations;

namespace BookMarkBrain.MVC.Models.TweetCategory;

public class TweetCategoryCreateViewModel
{
    [Required(ErrorMessage = "Tweet is required")]
    public Guid TweetId { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    // Used for dropdown selections
    public List<SelectItemViewModel> AvailableTweets { get; set; } = new List<SelectItemViewModel>();
    public List<SelectItemViewModel> AvailableCategories { get; set; } = new List<SelectItemViewModel>();
}
