namespace BookMarkBrain.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string ColorHex { get; set; }

    public int DisplayOrder { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public Category ParentCategory { get; set; }

    public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
    public ICollection<TweetCategory> TweetCategories { get; set; } = new List<TweetCategory>();
}


