namespace FoodieApp.ViewModels;

[QueryProperty(nameof(RecipeId), "recipeId")]
public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly ISettingsService _settingsService;
    private CancellationTokenSource? _ttsCts;

    public RecipeDetailViewModel(IRecipeService recipeService, ISettingsService settingsService)
    {
        _recipeService = recipeService;
        _settingsService = settingsService;
        Title = "Recipe Details";
    }

    [ObservableProperty]
    private int _recipeId;

    [ObservableProperty]
    private Recipe? _recipe;

    [ObservableProperty]
    private bool _isFavourite;

    [ObservableProperty]
    private string _servingsText = "4";

    [ObservableProperty]
    private bool _isSpeaking;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private string _speakButtonText = "🔊";

    partial void OnRecipeIdChanged(int value)
    {
        LoadRecipeCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadRecipeAsync()
    {
        await ExecuteAsync(async () =>
        {
            Recipe = await _recipeService.GetRecipeByIdAsync(RecipeId);
            if (Recipe != null)
            {
                Title = Recipe.Name;
                IsFavourite = Recipe.IsFavourite;
                ServingsText = Recipe.Servings.ToString();
            }
        }, "Failed to load recipe");
    }

    [RelayCommand]
    private async Task ToggleFavouriteAsync()
    {
        if (Recipe == null) return;

        await ExecuteAsync(async () =>
        {
            await _recipeService.ToggleFavouriteAsync(Recipe.Id);
            IsFavourite = !IsFavourite;

            if (!_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformClick();
        }, "Failed to update favourite");
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task SpeakRecipeAsync()
    {
        if (Recipe == null) return;

        // Toggle: idle or paused → start/resume speaking
        if (!IsSpeaking || IsPaused)
        {
            // If paused, resume instead of restarting
            if (IsSpeaking && IsPaused)
            {
                await Helpers.TextToSpeechHelper.ResumeAsync();
                IsPaused = false;
                SpeakButtonText = "⏸";
                return;
            }

            // Start fresh
            await StartSpeakRecipeAsync();
            return;
        }

        // Toggle: speaking (not paused) → pause
        if (IsSpeaking && !IsPaused)
        {
            await Helpers.TextToSpeechHelper.PauseAsync();
            IsPaused = true;
            SpeakButtonText = "🔊";
            return;
        }
    }

    /// <summary>
    /// Starts reading the full recipe aloud (name + description + instructions).
    /// Called by the speak toggle and can be called externally.
    /// </summary>
    private async Task StartSpeakRecipeAsync()
    {
        try
        {
            // Clean up any previous session
            _ttsCts?.Cancel();
            CleanupTts();

            IsSpeaking = true;
            IsPaused = false;
            SpeakButtonText = "⏸";
            _ttsCts = new CancellationTokenSource();

            var textToSpeak = $"{Recipe.Name}. {Recipe.Description}. Next, instructions. " +
                string.Join(". Next step. ", Recipe.Instructions);

            await Helpers.TextToSpeechHelper.SpeakAsync(textToSpeak, _ttsCts.Token);
        }
        catch (TaskCanceledException)
        {
            System.Diagnostics.Debug.WriteLine("TTS cancelled by user");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("TTS Error", ex.Message, "OK");
        }
        finally
        {
            // Only reset UI if not paused (paused = user intentionally paused)
            if (!Helpers.TextToSpeechHelper.IsPaused)
            {
                CleanupTts();
                SpeakButtonText = "🔊";
                IsSpeaking = false;
                IsPaused = false;
            }
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task SpeakNutritionAsync()
    {
        if (Recipe?.Nutrition == null) return;

        // Toggle: idle or paused → start/resume speaking
        if (!IsSpeaking || IsPaused)
        {
            if (IsSpeaking && IsPaused)
            {
                await Helpers.TextToSpeechHelper.ResumeAsync();
                IsPaused = false;
                SpeakButtonText = "⏸";
                return;
            }

            await StartSpeakNutritionAsync();
            return;
        }

        // Toggle: speaking (not paused) → pause
        if (IsSpeaking && !IsPaused)
        {
            await Helpers.TextToSpeechHelper.PauseAsync();
            IsPaused = true;
            SpeakButtonText = "🔊";
            return;
        }
    }

    /// <summary>
    /// Starts reading nutrition info aloud.
    /// </summary>
    private async Task StartSpeakNutritionAsync()
    {
        try
        {
            _ttsCts?.Cancel();
            CleanupTts();

            IsSpeaking = true;
            IsPaused = false;
            SpeakButtonText = "⏸";
            _ttsCts = new CancellationTokenSource();

            var n = Recipe.Nutrition;
            var text = $"Nutrition per serving. " +
                       $"Calories: {n.Calories:F0}. " +
                       $"Protein: {n.ProteinGrams:F1} grams. " +
                       $"Carbohydrates: {n.CarbohydratesGrams:F1} grams. " +
                       $"Fat: {n.FatGrams:F1} grams. " +
                       $"Fiber: {n.FiberGrams:F1} grams. " +
                       $"Sugar: {n.SugarGrams:F1} grams. " +
                       $"Sodium: {n.SodiumMilligrams:F0} milligrams.";

            await Helpers.TextToSpeechHelper.SpeakAsync(text, _ttsCts.Token);
        }
        catch (TaskCanceledException)
        {
            System.Diagnostics.Debug.WriteLine("TTS nutrition cancelled by user");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("TTS Error", ex.Message, "OK");
        }
        finally
        {
            if (!Helpers.TextToSpeechHelper.IsPaused)
            {
                CleanupTts();
                SpeakButtonText = "🔊";
                IsSpeaking = false;
                IsPaused = false;
            }
        }
    }

    [RelayCommand]
    private async Task ShareRecipeAsync()
    {
        if (Recipe == null) return;

        try
        {
            var shareText = $"Check out this recipe: {Recipe.Name}\n\n" +
                $"Ingredients:\n{string.Join("\n", Recipe.Ingredients)}\n\n" +
                $"Instructions:\n{string.Join("\n", Recipe.Instructions)}";

            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = Recipe.Name,
                Text = shareText
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Share error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void UpdateServings(string adjustment)
    {
        if (Recipe == null) return;

        try
        {
            if (!int.TryParse(ServingsText, out int currentServings))
            {
                ServingsText = Recipe.Servings.ToString();
                return;
            }

            int newServings = adjustment == "increase" ? currentServings + 1 : currentServings - 1;
            newServings = Math.Max(1, newServings);
            ServingsText = newServings.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Servings update error: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops any ongoing TTS and resets state. Call when leaving the page.
    /// </summary>
    public async Task StopSpeakingAsync()
    {
        _ttsCts?.Cancel();
        await Helpers.TextToSpeechHelper.StopAsync();
        CleanupTts();
        SpeakButtonText = "🔊";
        IsSpeaking = false;
        IsPaused = false;
    }

    private void CleanupTts()
    {
        _ttsCts?.Dispose();
        _ttsCts = null;
        _isPaused = false;
    }
}
