namespace FoodieApp.Views;

public partial class RestaurantFinderPage : ContentPage
{
    public RestaurantFinderPage(RestaurantFinderViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
