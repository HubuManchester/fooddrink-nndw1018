namespace FoodieApp.Services;

public class BarcodeService : IBarcodeService
{
    private bool _isScanning;

    public async Task<string> ScanBarcodeAsync()
    {
        if (_isScanning)
        {
            return string.Empty;
        }

        try
        {
            _isScanning = true;

            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    return string.Empty;
                }
            }

            return await Task.FromResult(string.Empty);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Barcode scan error: {ex.Message}");
            return string.Empty;
        }
        finally
        {
            _isScanning = false;
        }
    }

    public bool IsBarcodeValid(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return false;
        }

        if (barcode.Length < 8 || barcode.Length > 14)
        {
            return false;
        }

        if (!barcode.All(char.IsDigit))
        {
            return false;
        }

        return true;
    }
}
