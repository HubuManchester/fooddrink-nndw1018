namespace FoodieApp.ViewModels;

public partial class BarcodeScanViewModel : BaseViewModel
{
    private readonly INutritionService _nutritionService;
    private readonly ISettingsService _settingsService;

    public BarcodeScanViewModel(INutritionService nutritionService, ISettingsService settingsService)
    {
        _nutritionService = nutritionService;
        _settingsService = settingsService;
        Title = "Barcode Scanner";
    }

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private BarcodeProduct? _scannedProduct;

    [ObservableProperty]
    private string _manualBarcode = string.Empty;

    [ObservableProperty]
    private bool _hasScanned;

    [ObservableProperty]
    private Label _barcodeLabel = new();

    [RelayCommand]
    private async Task StartScanningAsync()
    {
        try
        {
#if WINDOWS
            // On Windows, check if MediaCapture is available first
            var mediaCapture = new Windows.Media.Capture.MediaCapture();
            try
            {
                await mediaCapture.InitializeAsync();
            }
            catch (UnauthorizedAccessException)
            {
                SetError("Camera access is blocked by Windows. Please enable 'Let desktop apps access your camera' in Windows Settings > Privacy & Security > Camera.");
                return;
            }
            catch (Exception mediaEx) when (mediaEx.Message.Contains("camera", StringComparison.OrdinalIgnoreCase))
            {
                SetError($"Cannot access camera. Make sure a camera is connected and Windows privacy settings allow camera access.\n\nDetails: {mediaEx.Message}");
                return;
            }
            finally
            {
                mediaCapture.Dispose();
            }
#endif

            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
#if WINDOWS
                    SetError("Camera permission denied. Please enable 'Camera access' for this app in Windows Settings > Privacy & Security > Camera.");
#else
                    SetError(Constants.ErrorMessages.CameraPermissionDenied);
#endif
                    return;
                }
            }

            IsScanning = true;
            HasScanned = false;
            ScannedProduct = null;
        }
        catch (Exception ex)
        {
            SetError($"Camera error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void StopScanning()
    {
        IsScanning = false;
    }

    [RelayCommand]
    private async Task LookupManualBarcodeAsync()
    {
        if (string.IsNullOrWhiteSpace(ManualBarcode))
        {
            SetError("Please enter a barcode number.");
            return;
        }

        if (ManualBarcode.Length < 8 || !ManualBarcode.All(char.IsDigit))
        {
            SetError("Please enter a valid barcode (8-14 digits).");
            return;
        }

        await LookupProductAsync(ManualBarcode);
    }

    public async Task OnBarcodeDetectedAsync(string barcode)
    {
        if (!IsScanning) return;

        IsScanning = false;
        await LookupProductAsync(barcode);
    }

    private async Task LookupProductAsync(string barcode)
    {
        await ExecuteAsync(async () =>
        {
            var product = await _nutritionService.GetProductByBarcodeAsync(barcode);
            ScannedProduct = product;
            HasScanned = true;

            if (product != null && !_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformClick();

            if (product == null)
            {
                SetError($"Product not found for barcode: {barcode}. Try a different barcode or enter manually.");
            }
        }, "Failed to lookup product");
    }

    [RelayCommand]
    private void ClearResult()
    {
        ScannedProduct = null;
        HasScanned = false;
        ManualBarcode = string.Empty;
        ClearError();
    }
}
