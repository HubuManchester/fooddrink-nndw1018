namespace FoodieApp.Models;

public class FoodCategory
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RecipeCount { get; set; }
}
