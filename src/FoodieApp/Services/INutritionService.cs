namespace FoodieApp.Services;

public interface INutritionService
{
    Task<BarcodeProduct?> GetProductByBarcodeAsync(string barcode);
    NutritionInfo CalculateDailyTotal(List<Recipe> recipes);
    bool IsAllergenPresent(BarcodeProduct product, List<string> userAllergens);
}
