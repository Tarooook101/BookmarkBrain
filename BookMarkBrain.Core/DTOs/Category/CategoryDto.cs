namespace BookMarkBrain.Core.DTOs.Category;
public class CategoryDto
{

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ColorHex { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Guid> ChildCategoryIds { get; set; } = new List<Guid>();
}
