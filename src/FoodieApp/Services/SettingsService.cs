namespace FoodieApp.Services;

public class SettingsService : ISettingsService
{
    private bool _isDarkMode;
    private double _fontScale = Constants.DefaultFontScale;
    private bool _reduceAnimations;
    private bool _highContrast;

    public event EventHandler? SettingsChanged;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                OnSettingsChanged();
            }
        }
    }

    public double FontScale
    {
        get => _fontScale;
        set
        {
            double clampedValue = Math.Clamp(value, Constants.MinFontScale, Constants.MaxFontScale);
            if (Math.Abs(_fontScale - clampedValue) > 0.01)
            {
                _fontScale = clampedValue;
                OnSettingsChanged();
            }
        }
    }

    public bool ReduceAnimations
    {
        get => _reduceAnimations;
        set
        {
            if (_reduceAnimations != value)
            {
                _reduceAnimations = value;
                OnSettingsChanged();
            }
        }
    }

    public bool HighContrast
    {
        get => _highContrast;
        set
        {
            if (_highContrast != value)
            {
                _highContrast = value;
                OnSettingsChanged();
            }
        }
    }

    public void ApplyAccessibilitySettings()
    {
        try
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = _isDarkMode ? AppTheme.Dark : AppTheme.Light;

                // Apply font scale to dynamic resources — all labels/styles
                // using {DynamicResource XXFontSize} will update automatically
                Application.Current.Resources["BaseFontSize"] = 14.0 * _fontScale;
                Application.Current.Resources["SmallFontSize"] = 12.0 * _fontScale;
                Application.Current.Resources["MediumFontSize"] = 16.0 * _fontScale;
                Application.Current.Resources["LargeFontSize"] = 20.0 * _fontScale;
                Application.Current.Resources["TitleFontSize"] = 26.0 * _fontScale;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying settings: {ex.Message}");
        }
    }

    private void OnSettingsChanged()
    {
        SettingsChanged?.Invoke(this, EventArgs.Empty);
        ApplyAccessibilitySettings();
    }
}
