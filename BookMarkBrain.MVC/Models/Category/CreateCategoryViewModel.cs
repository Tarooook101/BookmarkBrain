using System.ComponentModel.DataAnnotations;

namespace BookMarkBrain.MVC.Models.Category;

public class CreateCategoryViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string Description { get; set; }

    [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex code (e.g., #FF5733)")]
    public string ColorHex { get; set; } = "#3498db";

    [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
    public int DisplayOrder { get; set; } = 0;

    public Guid? ParentCategoryId { get; set; }

    public List<CategoryDropdownItem> AvailableParentCategories { get; set; } = new List<CategoryDropdownItem>();
}