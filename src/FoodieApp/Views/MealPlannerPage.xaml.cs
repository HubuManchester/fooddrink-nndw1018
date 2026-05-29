namespace FoodieApp.Views;

public partial class MealPlannerPage : ContentPage
{
    private readonly MealPlannerViewModel _viewModel;

    public MealPlannerPage(MealPlannerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
