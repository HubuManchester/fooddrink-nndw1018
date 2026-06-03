using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace FoodieApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IRecipeService, RecipeService>();
        builder.Services.AddSingleton<INutritionService, NutritionService>();
        builder.Services.AddSingleton<IBarcodeService, BarcodeService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IFoodRecordService, FoodRecordService>();

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<RecipeListViewModel>();
        builder.Services.AddTransient<RecipeDetailViewModel>();
        builder.Services.AddSingleton<BarcodeScanViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<MealPlannerViewModel>();
        builder.Services.AddSingleton<RestaurantFinderViewModel>();
        builder.Services.AddTransient<AddFoodRecordViewModel>();
        builder.Services.AddSingleton<MyRecordsViewModel>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<RecipeListPage>();
        builder.Services.AddTransient<RecipeDetailPage>();
        builder.Services.AddSingleton<BarcodeScanPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<MealPlannerPage>();
        builder.Services.AddSingleton<RestaurantFinderPage>();
        builder.Services.AddTransient<AddFoodRecordPage>();
        builder.Services.AddSingleton<MyRecordsPage>();

        return builder.Build();
    }
}
