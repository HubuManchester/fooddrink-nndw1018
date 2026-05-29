namespace FoodieApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    protected async Task ExecuteAsync(Func<Task> operation, string errorContext = "")
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;
            await operation();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = string.IsNullOrEmpty(errorContext)
                ? $"An unexpected error occurred: {ex.Message}"
                : $"{errorContext}: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ErrorMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected void SetError(string message)
    {
        HasError = true;
        ErrorMessage = message;
    }

    protected void ClearError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
