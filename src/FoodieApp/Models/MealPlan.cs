namespace FoodieApp.Models;

public class MealPlan
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public MealType Type { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public int RecipeId { get; set; }
}

public enum MealType
{
    Breakfast,
    Lunch,
    Dinner,
    Snack
}
