using BookMarkBrain.Core.DTOs.Category;

namespace BookMarkBrain.MVC.Models.Category;

public class CategoryTreeViewModel
{
    public IReadOnlyList<CategoryTreeDto> Categories { get; set; }
}
