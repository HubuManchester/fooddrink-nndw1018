namespace FoodieApp.Services;

/// <summary>
/// Service for managing recipe data and categories.
/// Provides search, filtering, favourites, and random recipe access.
/// </summary>
public interface IRecipeService
{
    /// <summary>Returns all recipes in the database.</summary>
    Task<List<Recipe>> GetAllRecipesAsync();

    /// <summary>Returns recipes filtered by a specific category name.</summary>
    /// <param name="category">Case-insensitive category name (e.g., "Breakfast", "Desserts").</param>
    Task<List<Recipe>> GetRecipesByCategoryAsync(string category);

    /// <summary>Looks up a single recipe by its unique Id. Returns null if not found.</summary>
    Task<Recipe?> GetRecipeByIdAsync(int id);

    /// <summary>
    /// Searches recipes by name, description, ingredients, and dietary tags.
    /// Performs case-insensitive partial matching.
    /// </summary>
    /// <param name="query">Search term to match against recipe fields.</param>
    Task<List<Recipe>> SearchRecipesAsync(string query);

    /// <summary>Returns all recipes that the user has marked as favourite.</summary>
    Task<List<Recipe>> GetFavouriteRecipesAsync();

    /// <summary>Returns a randomly selected recipe from the database.</summary>
    Task<Recipe> GetRandomRecipeAsync();

    /// <summary>Returns all food categories with recipe counts.</summary>
    Task<List<FoodCategory>> GetCategoriesAsync();

    /// <summary>Toggles the favourite status for a recipe. If favourited, un-favourites it and vice versa.</summary>
    /// <param name="recipeId">The unique ID of the recipe.</param>
    Task ToggleFavouriteAsync(int recipeId);
}
