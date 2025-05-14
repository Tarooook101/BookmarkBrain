namespace BookMarkBrain.MVC.Models.Category;

public class CategoryDropdownItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Level { get; set; } = 0;
}
