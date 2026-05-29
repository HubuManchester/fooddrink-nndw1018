namespace FoodieApp.Services;

public interface IRecipeService
{
    Task<List<Recipe>> GetAllRecipesAsync();
    Task<List<Recipe>> GetRecipesByCategoryAsync(string category);
    Task<Recipe?> GetRecipeByIdAsync(int id);
    Task<List<Recipe>> SearchRecipesAsync(string query);
    Task<List<Recipe>> GetFavouriteRecipesAsync();
    Task<Recipe> GetRandomRecipeAsync();
    Task<List<FoodCategory>> GetCategoriesAsync();
    Task ToggleFavouriteAsync(int recipeId);
}
