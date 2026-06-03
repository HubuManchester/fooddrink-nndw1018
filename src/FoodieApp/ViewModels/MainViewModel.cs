namespace FoodieApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly ISettingsService _settingsService;
    private readonly RecipeListViewModel _recipeListViewModel;
    private bool _isShakeDetecting;
    private DateTime _lastShakeTime = DateTime.MinValue;

    // Shake detection: threshold in G-force (1G ≈ 9.81 m/s²)
    private const double ShakeThresholdG = 1.5;
    private const int ShakeCooldownMs = 2000;

    public MainViewModel(IRecipeService recipeService, ISettingsService settingsService, RecipeListViewModel recipeListViewModel)
    {
        _recipeService = recipeService;
        _settingsService = settingsService;
        _recipeListViewModel = recipeListViewModel;
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
    }

    [RelayCommand]
    private void ClearSearch()
    {
        IsSearching = false;
        SearchQuery = string.Empty;
        SearchResults.Clear();
        ClearError();
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
            // Set the category directly on the RecipeListViewModel (singleton)
            // then switch to the Recipes tab. Avoids Shell query parameter bugs.
            _recipeListViewModel.Category = category.Name;
            await Shell.Current.GoToAsync($"//{nameof(RecipeListPage)}");
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

    [RelayCommand]
    private async Task NavigateToAddFoodRecordAsync()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(AddFoodRecordPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task NavigateToRestaurantFinderAsync()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(RestaurantFinderPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Starts listening to the accelerometer for shake detection.
    /// When a shake is detected, triggers the random recipe command.
    /// </summary>
    public void StartShakeDetection()
    {
        if (_isShakeDetecting) return;

        try
        {
            if (Accelerometer.Default.IsSupported)
            {
                Accelerometer.Default.ShakeDetected += OnShakeDetected;
                Accelerometer.Default.Start(SensorSpeed.Game);
                _isShakeDetecting = true;
                System.Diagnostics.Debug.WriteLine("Shake detection started (Accelerometer)");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Accelerometer not supported on this device");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to start accelerometer: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops accelerometer listening. Called when navigating away from the page.
    /// </summary>
    public void StopShakeDetection()
    {
        if (!_isShakeDetecting) return;

        try
        {
            Accelerometer.Default.ShakeDetected -= OnShakeDetected;
            Accelerometer.Default.Stop();
            _isShakeDetecting = false;
            System.Diagnostics.Debug.WriteLine("Shake detection stopped");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to stop accelerometer: {ex.Message}");
        }
    }

    private void OnShakeDetected(object sender, EventArgs e)
    {
        // Cooldown to prevent multiple triggers from a single shake
        var now = DateTime.Now;
        if ((now - _lastShakeTime).TotalMilliseconds < ShakeCooldownMs)
            return;
        _lastShakeTime = now;

        // Execute on main thread for UI updates
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await ShakeForRandomRecipeAsync();

                if (!_settingsService.ReduceAnimations)
                    HapticFeedbackHelper.PerformClick();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Shake handler error: {ex.Message}");
            }
        });
    }
}
