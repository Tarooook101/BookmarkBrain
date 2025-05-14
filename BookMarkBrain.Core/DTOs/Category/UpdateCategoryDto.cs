namespace BookMarkBrain.Core.DTOs.Category;
public class UpdateCategoryDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string ColorHex { get; set; }

    public int DisplayOrder { get; set; }

    public Guid? ParentCategoryId { get; set; }
}