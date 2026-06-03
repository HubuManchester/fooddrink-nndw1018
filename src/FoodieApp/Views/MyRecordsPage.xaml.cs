namespace FoodieApp.Views;

public partial class MyRecordsPage : ContentPage
{
    private readonly MyRecordsViewModel _viewModel;

    public MyRecordsPage(MyRecordsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRecordsCommand.ExecuteAsync(null);
    }
}
