namespace FoodieApp.Helpers;

public static class Constants
{
    public const string AppName = "Foodie";
    public const string AppVersion = "1.0.0";
    public const int MaxSearchResults = 50;
    public const int ShakeThreshold = 800;
    public const double DefaultFontScale = 1.0;
    public const double MaxFontScale = 2.0;
    public const double MinFontScale = 0.8;

    public static class ErrorMessages
    {
        public const string GenericError = "Something went wrong. Please try again.";
        public const string NetworkError = "Unable to connect. Please check your internet connection.";
        public const string BarcodeNotFound = "Product not found for this barcode.";
        public const string InvalidInput = "Please check your input and try again.";
        public const string LocationDisabled = "Location services are disabled. Please enable them in Settings.";
        public const string CameraPermissionDenied = "Camera permission is required to scan barcodes.";
        public const string EmptySearchQuery = "Please enter a search term.";
    }

    public static class AccessibilityLabels
    {
        public const string SearchButton = "Search for recipes";
        public const string FavouriteButton = "Add to favourites";
        public const string ShareButton = "Share recipe";
        public const string ScanButton = "Scan barcode";
        public const string BackButton = "Go back";
        public const string SettingsButton = "Open settings";
    }
}
