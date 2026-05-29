namespace FoodieApp.Services;

public interface IBarcodeService
{
    Task<string> ScanBarcodeAsync();
    bool IsBarcodeValid(string barcode);
}
