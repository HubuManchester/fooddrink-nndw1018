namespace FoodieApp.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        Title = "Settings";
        _isDarkMode = _settingsService.IsDarkMode;
        _fontScale = _settingsService.FontScale;
        _reduceAnimations = _settingsService.ReduceAnimations;
        _highContrast = _settingsService.HighContrast;
        _isInitialized = true;
    }

    [ObservableProperty]
    private bool _isDarkMode;

    [ObservableProperty]
    private double _fontScale;

    [ObservableProperty]
    private bool _reduceAnimations;

    [ObservableProperty]
    private bool _highContrast;

    [ObservableProperty]
    private string _fontScaleDisplayText = "1.0x";

    private bool _isInitialized;

    partial void OnIsDarkModeChanged(bool value)
    {
        _settingsService.IsDarkMode = value;
        if (_isInitialized)
            HapticFeedbackHelper.PerformClick();
    }

    partial void OnFontScaleChanged(double value)
    {
        _settingsService.FontScale = value;
        FontScaleDisplayText = $"{value:F1}x";
        if (_isInitialized)
            HapticFeedbackHelper.PerformClick();
    }

    partial void OnReduceAnimationsChanged(bool value)
    {
        _settingsService.ReduceAnimations = value;
        if (_isInitialized)
            HapticFeedbackHelper.PerformClick();
    }

    partial void OnHighContrastChanged(bool value)
    {
        _settingsService.HighContrast = value;
        if (_isInitialized)
            HapticFeedbackHelper.PerformClick();
    }

    [RelayCommand]
    private async Task ShowAccessibilityInfoAsync()
    {
        try
        {
            await Shell.Current.DisplayAlert(
                "Accessibility Features",
                "• Dark Mode: Reduces eye strain in low light\n" +
                "• Font Scaling: Adjust text size to your preference\n" +
                "• Reduce Animations: Minimizes motion for vestibular disorders\n" +
                "• High Contrast: Increases contrast ratios for better readability\n\n" +
                "These features follow WCAG 2.1 guidelines for mobile accessibility.",
                "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dialog error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ShowHelpAsync()
    {
        try
        {
            await Shell.Current.DisplayAlert(
                $"How to Use {Constants.AppName}",
                "📋 Discover — Browse recipes by category, search, or shake your device for a random pick.\n\n" +
                "📖 Recipes — Filter and sort the full recipe list. Tap any item for details.\n\n" +
                "📷 Scan — Scan food barcodes for nutrition info, or enter codes manually.\n\n" +
                "📝 Records — Add food records with photos, GPS location, and nutrition data.\n\n" +
                "📅 Meals — Plan your weekly meals and view daily nutrition totals.\n\n" +
                "🗺️ Nearby — Find restaurants based on your location.\n\n" +
                "⚙️ Settings — Customise theme, font size, contrast, and accessibility options.",
                "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Help dialog error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SpeakAccessibilityInfoAsync()
    {
        try
        {
            var text = "Accessibility features. " +
                       "Dark Mode: Reduces eye strain in low light. " +
                       "Font Scaling: Adjust text size from 80 percent to 200 percent. " +
                       "Reduce Animations: Minimizes motion for vestibular disorders. " +
                       "High Contrast: Increases contrast ratios for better readability. " +
                       "These features follow W C A G 2.1 guidelines for mobile accessibility.";

            await Helpers.TextToSpeechHelper.SpeakAsync(text);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Text-to-Speech Error",
                $"Could not speak accessibility info: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ShowAboutAsync()
    {
        try
        {
            await Shell.Current.DisplayAlert(
                $"About {Constants.AppName}",
                $"Version {Constants.AppVersion}\n\n" +
                "Foodie is your ultimate food companion app. " +
                "Browse recipes, scan barcodes for nutrition info, " +
                "plan meals, and discover local restaurants.\n\n" +
                "Built with .NET MAUI for cross-platform mobile devices.",
                "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dialog error: {ex.Message}");
        }
    }
}
