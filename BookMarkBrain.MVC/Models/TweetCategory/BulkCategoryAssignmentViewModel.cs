using System.ComponentModel.DataAnnotations;

namespace BookMarkBrain.MVC.Models.TweetCategory;

public class BulkCategoryAssignmentViewModel
{
    [Required]
    public Guid TweetId { get; set; }
    public List<Guid> CategoryIds { get; set; } = new List<Guid>();
}