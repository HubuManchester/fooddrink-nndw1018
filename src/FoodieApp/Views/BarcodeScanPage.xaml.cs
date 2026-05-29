using ZXing.Net.Maui;

namespace FoodieApp.Views;

public partial class BarcodeScanPage : ContentPage
{
    private readonly BarcodeScanViewModel _viewModel;

    public BarcodeScanPage(BarcodeScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (e.Results.Length > 0)
        {
            var barcode = e.Results[0].Value;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _viewModel.OnBarcodeDetectedAsync(barcode);
            });
        }
    }
}
