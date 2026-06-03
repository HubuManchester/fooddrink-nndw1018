namespace FoodieApp.ViewModels;

public partial class MyRecordsViewModel : BaseViewModel
{
    private readonly IFoodRecordService _foodRecordService;

    public MyRecordsViewModel(IFoodRecordService foodRecordService)
    {
        _foodRecordService = foodRecordService;
        Title = "My Records";
    }

    [ObservableProperty]
    private ObservableCollection<FoodRecord> _records = new();

    [ObservableProperty]
    private bool _isEmpty;

    [RelayCommand]
    private async Task LoadRecordsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var list = await _foodRecordService.GetAllFoodRecordsAsync();
            Records = new ObservableCollection<FoodRecord>(list);
            IsEmpty = Records.Count == 0;
        }, "Failed to load food records");
    }

    [RelayCommand]
    private async Task ViewRecordDetailAsync(FoodRecord record)
    {
        if (record == null) return;

        try
        {
            var nutritionSummary = $"Calories: {record.Nutrition.Calories:F0} kcal\n" +
                                   $"Protein: {record.Nutrition.ProteinGrams:F1}g | " +
                                   $"Carbs: {record.Nutrition.CarbohydratesGrams:F1}g | " +
                                   $"Fat: {record.Nutrition.FatGrams:F1}g\n" +
                                   $"Fiber: {record.Nutrition.FiberGrams:F1}g | " +
                                   $"Sugar: {record.Nutrition.SugarGrams:F1}g | " +
                                   $"Sodium: {record.Nutrition.SodiumMilligrams:F0}mg";

            var locationInfo = (record.Latitude.HasValue && record.Longitude.HasValue)
                ? record.LocationName ?? $"{record.Latitude:F4}, {record.Longitude:F4}"
                : "No location recorded";

            var photoInfo = !string.IsNullOrWhiteSpace(record.PhotoFilePath)
                ? "Photo available"
                : "No photo";

            var message = $"Category: {record.Category}\n" +
                          $"Description: {record.Description}\n\n" +
                          $"Nutrition:\n{nutritionSummary}\n\n" +
                          $"Location: {locationInfo}\n" +
                          $"Photo: {photoInfo}\n\n" +
                          $"Created: {record.CreatedAt:yyyy-MM-dd HH:mm}";

            await Shell.Current.DisplayAlert(record.Name, message, "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"View record error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteRecordAsync(FoodRecord record)
    {
        if (record == null) return;

        try
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Record",
                $"Are you sure you want to delete \"{record.Name}\"?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            await _foodRecordService.DeleteFoodRecordAsync(record.Id);

            if (!string.IsNullOrWhiteSpace(record.PhotoFilePath) && File.Exists(record.PhotoFilePath))
            {
                try { File.Delete(record.PhotoFilePath); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Photo delete error: {ex.Message}"); }
            }

            await LoadRecordsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete record error: {ex.Message}");
            SetError("Failed to delete the record. Please try again.");
        }
    }

    [RelayCommand]
    private async Task NavigateToAddRecordAsync()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(AddFoodRecordPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }
}
