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
        Routing.RegisterRoute(nameof(AddFoodRecordPage), typeof(AddFoodRecordPage));
        Routing.RegisterRoute(nameof(MyRecordsPage), typeof(MyRecordsPage));
    }
}
