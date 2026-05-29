using FoodieApp.Helpers;

namespace FoodieApp;

public partial class App : Application
{
    private readonly ISettingsService _settingsService;

    public App(ISettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;
        ApplyTheme();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell(_settingsService));
    }

    private void ApplyTheme()
    {
        UserAppTheme = _settingsService.IsDarkMode ? AppTheme.Dark : AppTheme.Light;
    }
}
