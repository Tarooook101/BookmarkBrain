namespace BookMarkBrain.Core.DTOs.Category;

public class CategoryTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ColorHex { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<CategoryTreeDto> Children { get; set; } = new List<CategoryTreeDto>();
}