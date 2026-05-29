namespace FoodieApp.ViewModels;

[QueryProperty(nameof(RecipeId), "recipeId")]
public partial class RecipeDetailViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;

    public RecipeDetailViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
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
        }, "Failed to update favourite");
    }

    [RelayCommand]
    private async Task SpeakRecipeAsync()
    {
        if (Recipe == null) return;

        try
        {
            if (IsSpeaking)
            {
                IsSpeaking = false;
                return;
            }

            IsSpeaking = true;
            var textToSpeak = $"{Recipe.Name}. {Recipe.Description}. " +
                string.Join(". ", Recipe.Instructions);

            await TextToSpeech.Default.SpeakAsync(textToSpeak, new SpeechOptions
            {
                Pitch = 1.0f,
                Volume = 1.0f
            });

            IsSpeaking = false;
        }
        catch (Exception ex)
        {
            IsSpeaking = false;
            System.Diagnostics.Debug.WriteLine($"TTS error: {ex.Message}");
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

            await Share.Default.RequestAsync(new ShareRequest
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
}
