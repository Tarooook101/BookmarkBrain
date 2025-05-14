namespace BookMarkBrain.Core.DTOs.Collection;

public class CreateCollectionDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public bool IsPublic { get; set; }
    public int DisplayOrder { get; set; }
}
