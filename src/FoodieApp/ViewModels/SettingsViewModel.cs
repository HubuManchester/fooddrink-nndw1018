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

    partial void OnIsDarkModeChanged(bool value)
    {
        _settingsService.IsDarkMode = value;
    }

    partial void OnFontScaleChanged(double value)
    {
        _settingsService.FontScale = value;
        FontScaleDisplayText = $"{value:F1}x";
    }

    partial void OnReduceAnimationsChanged(bool value)
    {
        _settingsService.ReduceAnimations = value;
    }

    partial void OnHighContrastChanged(bool value)
    {
        _settingsService.HighContrast = value;
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
