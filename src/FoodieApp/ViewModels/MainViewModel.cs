namespace FoodieApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly ISettingsService _settingsService;

    public MainViewModel(IRecipeService recipeService, ISettingsService settingsService)
    {
        _recipeService = recipeService;
        _settingsService = settingsService;
        Title = "Discover";
    }

    [ObservableProperty]
    private ObservableCollection<FoodCategory> _categories = new();

    [ObservableProperty]
    private ObservableCollection<Recipe> _popularRecipes = new();

    [ObservableProperty]
    private Recipe? _recipeOfTheDay;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Recipe> _searchResults = new();

    [ObservableProperty]
    private bool _isSearching;

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var categories = await _recipeService.GetCategoriesAsync();
            Categories = new ObservableCollection<FoodCategory>(categories);

            var recipes = await _recipeService.GetAllRecipesAsync();
            var topRated = recipes.OrderByDescending(r => r.Rating).Take(5).ToList();
            PopularRecipes = new ObservableCollection<Recipe>(topRated);

            var random = new Random();
            int index = random.Next(recipes.Count);
            RecipeOfTheDay = recipes[index];
        }, "Failed to load discover page");
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            SetError(Constants.ErrorMessages.EmptySearchQuery);
            return;
        }

        await ExecuteAsync(async () =>
        {
            IsSearching = true;
            var results = await _recipeService.SearchRecipesAsync(SearchQuery);
            SearchResults = new ObservableCollection<Recipe>(results);

            if (results.Count == 0)
            {
                ErrorMessage = "No recipes found matching your search.";
                HasError = true;
            }
        }, "Search failed");

        IsSearching = false;
    }

    [RelayCommand]
    private async Task ShakeForRandomRecipeAsync()
    {
        await ExecuteAsync(async () =>
        {
            var recipe = await _recipeService.GetRandomRecipeAsync();
            RecipeOfTheDay = recipe;
        }, "Failed to get random recipe");
    }

    [RelayCommand]
    private async Task NavigateToCategoryAsync(FoodCategory category)
    {
        if (category == null) return;

        try
        {
            await Shell.Current.GoToAsync($"{nameof(RecipeListPage)}?category={Uri.EscapeDataString(category.Name)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task NavigateToRecipeDetailAsync(Recipe recipe)
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
}
