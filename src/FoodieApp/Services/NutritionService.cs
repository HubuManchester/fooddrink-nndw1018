namespace FoodieApp.Services;

public class NutritionService : INutritionService
{
    private readonly Dictionary<string, BarcodeProduct> _productDatabase;

    public NutritionService()
    {
        _productDatabase = new Dictionary<string, BarcodeProduct>
        {
            ["5000159473522"] = new BarcodeProduct
            {
                Barcode = "5000159473522",
                ProductName = "Heinz Baked Beans",
                Brand = "Heinz",
                Nutrition = new NutritionInfo { Calories = 78, ProteinGrams = 4.7, CarbohydratesGrams = 12.5, FatGrams = 0.2, FiberGrams = 3.7, SugarGrams = 4.7, SodiumMilligrams = 270 },
                Ingredients = new List<string> { "Beans (51%)", "Tomatoes (34%)", "Water", "Sugar", "Modified Cornflour", "Salt", "Spice Extracts" },
                Allergens = new List<string>()
            },
            ["5000112636147"] = new BarcodeProduct
            {
                Barcode = "5000112636147",
                ProductName = "Coca-Cola Original 330ml",
                Brand = "Coca-Cola",
                Nutrition = new NutritionInfo { Calories = 139, ProteinGrams = 0, CarbohydratesGrams = 35, FatGrams = 0, FiberGrams = 0, SugarGrams = 35, SodiumMilligrams = 0 },
                Ingredients = new List<string> { "Carbonated Water", "Sugar", "Colour (Caramel E150d)", "Phosphoric Acid", "Natural Flavourings", "Caffeine" },
                Allergens = new List<string>()
            },
            ["5000168100482"] = new BarcodeProduct
            {
                Barcode = "5000168100482",
                ProductName = "Walkers Ready Salted Crisps",
                Brand = "Walkers",
                Nutrition = new NutritionInfo { Calories = 132, ProteinGrams = 1.3, CarbohydratesGrams = 13.5, FatGrams = 8.2, FiberGrams = 0.7, SugarGrams = 0.2, SodiumMilligrams = 200 },
                Ingredients = new List<string> { "Potatoes", "Sunflower Oil", "Salt" },
                Allergens = new List<string>()
            },
            ["5012035100227"] = new BarcodeProduct
            {
                Barcode = "5012035100227",
                ProductName = "Green & Black's Organic Dark Chocolate",
                Brand = "Green & Black's",
                Nutrition = new NutritionInfo { Calories = 542, ProteinGrams = 7.3, CarbohydratesGrams = 37.9, FatGrams = 38.2, FiberGrams = 9.6, SugarGrams = 28.1, SodiumMilligrams = 10 },
                Ingredients = new List<string> { "Organic Cocoa Mass", "Organic Cane Sugar", "Organic Cocoa Butter", "Organic Vanilla Extract" },
                Allergens = new List<string> { "May contain milk" }
            }
        };
    }

    public Task<BarcodeProduct?> GetProductByBarcodeAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return Task.FromResult<BarcodeProduct?>(null);
        }

        try
        {
            if (_productDatabase.TryGetValue(barcode, out var product))
            {
                return Task.FromResult<BarcodeProduct?>(product);
            }

            return Task.FromResult<BarcodeProduct?>(null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error looking up barcode {barcode}: {ex.Message}");
            return Task.FromResult<BarcodeProduct?>(null);
        }
    }

    public NutritionInfo CalculateDailyTotal(List<Recipe> recipes)
    {
        if (recipes == null || recipes.Count == 0)
        {
            return new NutritionInfo();
        }

        try
        {
            var total = new NutritionInfo();
            foreach (var recipe in recipes)
            {
                total.Calories += recipe.Nutrition.Calories;
                total.ProteinGrams += recipe.Nutrition.ProteinGrams;
                total.CarbohydratesGrams += recipe.Nutrition.CarbohydratesGrams;
                total.FatGrams += recipe.Nutrition.FatGrams;
                total.FiberGrams += recipe.Nutrition.FiberGrams;
                total.SugarGrams += recipe.Nutrition.SugarGrams;
                total.SodiumMilligrams += recipe.Nutrition.SodiumMilligrams;
            }

            return total;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error calculating nutrition: {ex.Message}");
            return new NutritionInfo();
        }
    }

    public bool IsAllergenPresent(BarcodeProduct product, List<string> userAllergens)
    {
        if (product == null || userAllergens == null || userAllergens.Count == 0)
        {
            return false;
        }

        try
        {
            return product.Allergens.Any(allergen =>
                userAllergens.Any(ua =>
                    allergen.Contains(ua, StringComparison.OrdinalIgnoreCase)));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking allergens: {ex.Message}");
            return false;
        }
    }
}
