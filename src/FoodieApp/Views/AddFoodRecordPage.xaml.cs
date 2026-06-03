namespace FoodieApp.Views;

public partial class AddFoodRecordPage : ContentPage
{
    private readonly AddFoodRecordViewModel _viewModel;

    public AddFoodRecordPage(AddFoodRecordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCategoriesCommand.ExecuteAsync(null);
    }

#pragma warning disable FQA0012 // async void required for event handler
    private async void OnCategoryPickerTapped(object sender, TappedEventArgs e)
    {
        if (_viewModel.Categories.Count == 0) return;

        var result = await DisplayActionSheet(
            "Select Category", "Cancel", null,
            _viewModel.Categories.ToArray());

        if (!string.IsNullOrWhiteSpace(result) && result != "Cancel")
        {
            _viewModel.SelectedCategory = result;
        }
    }
#pragma warning restore FQA0012
}
