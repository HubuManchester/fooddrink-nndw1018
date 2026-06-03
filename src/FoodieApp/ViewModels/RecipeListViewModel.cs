namespace FoodieApp.ViewModels;

[QueryProperty(nameof(Category), "category")]
public partial class RecipeListViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly ISettingsService _settingsService;

    public RecipeListViewModel(IRecipeService recipeService, ISettingsService settingsService)
    {
        _recipeService = recipeService;
        _settingsService = settingsService;
        Title = "Recipes";
    }

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Recipe> _recipes = new();

    [ObservableProperty]
    private string _filterDifficulty = string.Empty;

    [ObservableProperty]
    private bool _showFavouritesOnly;

    [ObservableProperty]
    private string _sortOption = "Rating";

    public List<string> SortOptions { get; } = new() { "Rating", "Name", "Prep Time", "Calories" };
    public List<string> DifficultyOptions { get; } = new() { "All", "Easy", "Medium", "Hard" };

    partial void OnCategoryChanged(string value)
    {
        LoadRecipesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        await ExecuteAsync(async () =>
        {
            List<Recipe> recipeList;

            if (!string.IsNullOrWhiteSpace(Category) && Category != "All")
            {
                recipeList = await _recipeService.GetRecipesByCategoryAsync(Category);
            }
            else
            {
                recipeList = await _recipeService.GetAllRecipesAsync();
            }

            if (ShowFavouritesOnly)
            {
                recipeList = recipeList.Where(r => r.IsFavourite).ToList();
            }

            if (!string.IsNullOrWhiteSpace(FilterDifficulty) && FilterDifficulty != "All")
            {
                recipeList = recipeList
                    .Where(r => r.Difficulty.Equals(FilterDifficulty, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            recipeList = SortOption switch
            {
                "Name" => recipeList.OrderBy(r => r.Name).ToList(),
                "Prep Time" => recipeList.OrderBy(r => r.PrepTimeMinutes).ToList(),
                "Calories" => recipeList.OrderBy(r => r.Nutrition.Calories).ToList(),
                _ => recipeList.OrderByDescending(r => r.Rating).ToList()
            };

            Recipes = new ObservableCollection<Recipe>(recipeList);
        }, "Failed to load recipes");
    }

    [RelayCommand]
    private async Task ToggleFavouriteAsync(Recipe recipe)
    {
        if (recipe == null) return;

        await ExecuteAsync(async () =>
        {
            await _recipeService.ToggleFavouriteAsync(recipe.Id);

            if (!_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformClick();
        }, "Failed to update favourite");

        LoadRecipesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(Recipe recipe)
    {
        if (recipe == null) return;

        try
        {
            await Shell.Current.GoToAsync($"{nameof(RecipeDetailPage)}?recipeId={recipe.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ApplyFiltersAsync()
    {
        await LoadRecipesAsync();
    }
}
