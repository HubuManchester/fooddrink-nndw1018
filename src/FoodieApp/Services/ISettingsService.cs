namespace FoodieApp.Services;

public interface ISettingsService
{
    bool IsDarkMode { get; set; }
    double FontScale { get; set; }
    bool ReduceAnimations { get; set; }
    bool HighContrast { get; set; }
    event EventHandler? SettingsChanged;
    void ApplyAccessibilitySettings();
}
