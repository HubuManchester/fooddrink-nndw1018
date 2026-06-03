namespace FoodieApp;

public partial class App : Application
{
    private readonly ISettingsService _settingsService;

    public App(ISettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;
        _settingsService.ApplyAccessibilitySettings();
        _settingsService.SettingsChanged += OnSettingsChanged;
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var shell = new AppShell(_settingsService);
        shell.Navigated += (_, _) =>
        {
            var scale = _settingsService.FontScale;
            if (shell.CurrentPage != null)
                Helpers.FontScaleHelper.ApplyScale(shell.CurrentPage, scale);
        };
        return new Window(shell);
    }

    private void OnSettingsChanged(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _settingsService.ApplyAccessibilitySettings();

            // Walk the entire visual tree from the shell to apply font scale
            if (Windows.Count > 0 && Windows[0].Page != null)
            {
                var scale = _settingsService.FontScale;
                Helpers.FontScaleHelper.ApplyScale(Windows[0].Page, scale);
            }
        });
    }
}
