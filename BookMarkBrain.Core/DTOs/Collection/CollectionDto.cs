namespace BookMarkBrain.Core.DTOs.Collection;

public class CollectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}