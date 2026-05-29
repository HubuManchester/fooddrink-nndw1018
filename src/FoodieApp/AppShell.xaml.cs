namespace FoodieApp;

public partial class AppShell : Shell
{
    private readonly ISettingsService _settingsService;

    public AppShell(ISettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;

        Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
        Routing.RegisterRoute(nameof(RestaurantFinderPage), typeof(RestaurantFinderPage));
    }
}
