namespace FoodieApp.Models;

public class BarcodeProduct
{
    public string Barcode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public NutritionInfo Nutrition { get; set; } = new();
    public List<string> Ingredients { get; set; } = new();
    public List<string> Allergens { get; set; } = new();
}
