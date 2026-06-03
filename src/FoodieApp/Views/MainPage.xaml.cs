namespace FoodieApp.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        _viewModel.StartShakeDetection();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopShakeDetection();
    }

    private void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        if (sender is TapGestureRecognizer tgr &&
            tgr.Parent is Border border &&
            border.BindingContext is FoodCategory category)
        {
            _viewModel.NavigateToCategoryCommand.Execute(category);
        }
    }
}
