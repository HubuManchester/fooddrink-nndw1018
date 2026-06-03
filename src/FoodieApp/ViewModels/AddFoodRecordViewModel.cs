namespace FoodieApp.ViewModels;

public partial class AddFoodRecordViewModel : BaseViewModel
{
    private readonly IFoodRecordService _foodRecordService;
    private readonly ISettingsService _settingsService;
    private readonly IRecipeService _recipeService;
    private bool _isInitialized;

    public AddFoodRecordViewModel(
        IFoodRecordService foodRecordService,
        ISettingsService settingsService,
        IRecipeService recipeService)
    {
        _foodRecordService = foodRecordService;
        _settingsService = settingsService;
        _recipeService = recipeService;
        Title = "Add Food Record";
    }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string? _photoFilePath;

    [ObservableProperty]
    private bool _hasPhoto;

    [ObservableProperty]
    private double? _latitude;

    [ObservableProperty]
    private double? _longitude;

    [ObservableProperty]
    private string? _locationName;

    [ObservableProperty]
    private bool _hasLocation;

    [ObservableProperty]
    private bool _isRecordingLocation;

    [ObservableProperty]
    private ObservableCollection<string> _categories = new();

    [ObservableProperty]
    private string _caloriesText = "0";

    [ObservableProperty]
    private string _proteinText = "0";

    [ObservableProperty]
    private string _carbsText = "0";

    [ObservableProperty]
    private string _fatText = "0";

    [ObservableProperty]
    private string _fiberText = "0";

    [ObservableProperty]
    private string _sugarText = "0";

    [ObservableProperty]
    private string _sodiumText = "0";

    [ObservableProperty]
    private ObservableCollection<string> _fieldErrors = new();

    // Per-field error states
    [ObservableProperty]
    private bool _nameHasError;

    [ObservableProperty]
    private bool _categoryHasError;

    [ObservableProperty]
    private bool _caloriesHasError;

    [ObservableProperty]
    private bool _proteinHasError;

    [ObservableProperty]
    private bool _carbsHasError;

    [ObservableProperty]
    private bool _fatHasError;

    [ObservableProperty]
    private bool _fiberHasError;

    [ObservableProperty]
    private bool _sugarHasError;

    [ObservableProperty]
    private bool _sodiumHasError;

    partial void OnPhotoFilePathChanged(string? value)
    {
        HasPhoto = !string.IsNullOrWhiteSpace(value);
    }

    partial void OnLatitudeChanged(double? value)
    {
        HasLocation = value.HasValue && Longitude.HasValue;
    }

    partial void OnLongitudeChanged(double? value)
    {
        HasLocation = value.HasValue && Latitude.HasValue;
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        try
        {
            var catList = await _recipeService.GetCategoriesAsync();
            Categories = new ObservableCollection<string>(
                catList.Select(c => c.Name));
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load categories error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        try
        {
            var permissionError = await HardwareHelper.CheckCameraPermissionAsync();
            if (permissionError != null)
            {
                SetError(permissionError);
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return;

            var fileName = $"food_{DateTime.Now:yyyyMMddHHmmss}.jpg";
            var destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            using var sourceStream = await photo.OpenReadAsync();
            using var destStream = File.OpenWrite(destinationPath);
            await sourceStream.CopyToAsync(destStream);

            PhotoFilePath = destinationPath;

            if (!_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformClick();
        }
        catch (PermissionException)
        {
            SetError(Constants.ErrorMessages.CameraPermissionDenied);
        }
        catch (FeatureNotSupportedException)
        {
            SetError(HardwareHelper.GetUnavailableMessage("camera"));
        }
        catch (Exception ex)
        {
            SetError($"Failed to take photo. Please ensure your camera is not in use by another app.");
            System.Diagnostics.Debug.WriteLine($"Photo error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ClearPhoto()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(PhotoFilePath) && File.Exists(PhotoFilePath))
            {
                File.Delete(PhotoFilePath);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete photo error: {ex.Message}");
        }

        PhotoFilePath = null;
    }

    [RelayCommand]
    private async Task RecordLocationAsync()
    {
        try
        {
            IsRecordingLocation = true;
            ClearError();

            var permissionError = await HardwareHelper.CheckLocationPermissionAsync();
            if (permissionError != null)
            {
                SetError(permissionError);
                return;
            }

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location == null)
            {
                SetError("Unable to get your location. Please ensure GPS is enabled and try again.");
                return;
            }

            Latitude = location.Latitude;
            Longitude = location.Longitude;

            try
            {
                LocationName = await GetAddressFromCoordinatesAsync(
                    location.Latitude, location.Longitude);
            }
            catch (Exception geoEx)
            {
                System.Diagnostics.Debug.WriteLine($"[Geocoding] {geoEx}");
                LocationName = $"{location.Latitude:F4}, {location.Longitude:F4}";
            }

            if (!_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformClick();
        }
        catch (PermissionException)
        {
            SetError(Constants.ErrorMessages.LocationDisabled);
        }
        catch (Exception ex)
        {
            SetError($"Failed to record location: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Location error: {ex.Message}");
        }
        finally
        {
            IsRecordingLocation = false;
        }
    }

    [RelayCommand]
    private void ClearLocation()
    {
        Latitude = null;
        Longitude = null;
        LocationName = null;
    }

    [RelayCommand]
    private async Task SaveRecordAsync()
    {
        if (!Validate()) return;

        await ExecuteAsync(async () =>
        {
            var record = new FoodRecord
            {
                Name = Name.Trim(),
                Category = SelectedCategory,
                Description = Description?.Trim() ?? string.Empty,
                PhotoFilePath = PhotoFilePath,
                Latitude = Latitude,
                Longitude = Longitude,
                LocationName = LocationName,
                Nutrition = new NutritionInfo
                {
                    Calories = ParseNutritionValue(CaloriesText),
                    ProteinGrams = ParseNutritionValue(ProteinText),
                    CarbohydratesGrams = ParseNutritionValue(CarbsText),
                    FatGrams = ParseNutritionValue(FatText),
                    FiberGrams = ParseNutritionValue(FiberText),
                    SugarGrams = ParseNutritionValue(SugarText),
                    SodiumMilligrams = ParseNutritionValue(SodiumText)
                }
            };

            await _foodRecordService.SaveFoodRecordAsync(record);

            if (!_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformClick();

            ResetForm();
            await Shell.Current.DisplayAlert("Success", "Food record saved successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }, "Failed to save food record");
    }

    private bool Validate()
    {
        ClearError();
        FieldErrors.Clear();
        ClearFieldErrors();
        bool isValid = true;

        // Name validation
        if (string.IsNullOrWhiteSpace(Name))
        {
            FieldErrors.Add("Name is required.");
            NameHasError = true;
            isValid = false;
        }

        // Category validation
        if (string.IsNullOrWhiteSpace(SelectedCategory))
        {
            FieldErrors.Add("Please select a category.");
            CategoryHasError = true;
            isValid = false;
        }

        // Nutrition per-field validation
        if (!double.TryParse(CaloriesText, out double calories) || calories < 0 || calories > 5000)
        {
            FieldErrors.Add("Calories must be between 0 and 5000.");
            CaloriesHasError = true;
            isValid = false;
        }

        if (!double.TryParse(ProteinText, out double protein) || protein < 0 || protein > 1000)
        {
            FieldErrors.Add("Protein must be between 0 and 1000 g.");
            ProteinHasError = true;
            isValid = false;
        }

        if (!double.TryParse(CarbsText, out double carbs) || carbs < 0 || carbs > 1000)
        {
            FieldErrors.Add("Carbohydrates must be between 0 and 1000 g.");
            CarbsHasError = true;
            isValid = false;
        }

        if (!double.TryParse(FatText, out double fat) || fat < 0 || fat > 1000)
        {
            FieldErrors.Add("Fat must be between 0 and 1000 g.");
            FatHasError = true;
            isValid = false;
        }

        if (!double.TryParse(FiberText, out double fiber) || fiber < 0 || fiber > 500)
        {
            FieldErrors.Add("Fiber must be between 0 and 500 g.");
            FiberHasError = true;
            isValid = false;
        }

        if (!double.TryParse(SugarText, out double sugar) || sugar < 0 || sugar > 500)
        {
            FieldErrors.Add("Sugar must be between 0 and 500 g.");
            SugarHasError = true;
            isValid = false;
        }

        if (!double.TryParse(SodiumText, out double sodium) || sodium < 0 || sodium > 10000)
        {
            FieldErrors.Add("Sodium must be between 0 and 10000 mg.");
            SodiumHasError = true;
            isValid = false;
        }

        if (!isValid)
        {
            HasError = true;
            ErrorMessage = "Please correct the highlighted fields below.";

            if (!_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformLongPress();
        }

        return isValid;
    }

    private void ClearFieldErrors()
    {
        NameHasError = false;
        CategoryHasError = false;
        CaloriesHasError = false;
        ProteinHasError = false;
        CarbsHasError = false;
        FatHasError = false;
        FiberHasError = false;
        SugarHasError = false;
        SodiumHasError = false;
    }

    private static double ParseNutritionValue(string text)
    {
        if (double.TryParse(text, out double value))
            return Math.Max(0, value);
        return 0;
    }

    private void ResetForm()
    {
        Name = string.Empty;
        SelectedCategory = string.Empty;
        Description = string.Empty;
        ClearPhotoCommand.Execute(null);
        ClearLocationCommand.Execute(null);
        CaloriesText = "0";
        ProteinText = "0";
        CarbsText = "0";
        FatText = "0";
        FiberText = "0";
        SugarText = "0";
        SodiumText = "0";
        FieldErrors.Clear();
        ClearError();
    }

    private static async Task<string> GetAddressFromCoordinatesAsync(double lat, double lon)
    {
        // Try 1: BigDataCloud (free, no key, works globally including China)
        try
        {
            var r = await GeocodeViaBigDataCloudAsync(lat, lon);
            if (r != null) return r;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BigDataCloud] {ex.Message}");
        }

        // Try 2: Nominatim OpenStreetMap (free, no key)
        try
        {
            var r = await GeocodeViaNominatimAsync(lat, lon);
            if (r != null) return r;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Nominatim] {ex.Message}");
        }

        // Fallback
        string ns = lat >= 0 ? "N" : "S";
        string ew = lon >= 0 ? "E" : "W";
        return $"{Math.Abs(lat):F4}°{ns}, {Math.Abs(lon):F4}°{ew}";
    }

    private static async Task<string?> GeocodeViaNominatimAsync(double lat, double lon)
    {
        var url = $"https://nominatim.openstreetmap.org/reverse" +
                  $"?lat={lat:F6}&lon={lon:F6}&format=json&accept-language=en&zoom=18";

        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(8);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 FoodieApp/1.0");

        var json = await client.GetStringAsync(url);
        var doc = System.Text.Json.JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("error", out var err))
            throw new Exception($"Nominatim: {err.GetString()}");

        var addr = doc.RootElement.TryGetProperty("address", out var a) ? a : default;
        if (addr.ValueKind != System.Text.Json.JsonValueKind.Object)
            return null;

        var parts = new List<string>();
        string[] keys = { "road", "suburb", "city_district", "city", "town",
                          "village", "county", "state_district", "state",
                          "postcode", "country" };
        foreach (var k in keys)
        {
            if (addr.TryGetProperty(k, out var v) &&
                !string.IsNullOrWhiteSpace(v.GetString()))
                parts.Add(v.GetString()!);
        }
        if (parts.Count > 0) return string.Join(", ", parts);

        if (doc.RootElement.TryGetProperty("display_name", out var dn) &&
            !string.IsNullOrWhiteSpace(dn.GetString()))
            return dn.GetString()!;

        return null;
    }

    private static async Task<string?> GeocodeViaBigDataCloudAsync(double lat, double lon)
    {
        var url = $"https://api.bigdatacloud.net/data/reverse-geocode-client" +
                  $"?latitude={lat:F6}&longitude={lon:F6}&localityLanguage=en";

        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(8);

        var json = await client.GetStringAsync(url);
        var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;

        var parts = new List<string>();

        // Street/district level
        if (root.TryGetProperty("locality", out var loc) && !string.IsNullOrWhiteSpace(loc.GetString()))
            parts.Add(loc.GetString()!);

        // City
        if (root.TryGetProperty("city", out var city) && !string.IsNullOrWhiteSpace(city.GetString()))
            parts.Add(city.GetString()!);

        // Province/State
        if (root.TryGetProperty("principalSubdivision", out var sub) && !string.IsNullOrWhiteSpace(sub.GetString()))
            parts.Add(sub.GetString()!);

        // Country
        if (root.TryGetProperty("countryName", out var cname) && !string.IsNullOrWhiteSpace(cname.GetString()))
            parts.Add(cname.GetString()!);

        // Postcode
        if (root.TryGetProperty("postcode", out var pc) && !string.IsNullOrWhiteSpace(pc.GetString()))
            parts.Add(pc.GetString()!);

        if (parts.Count > 0)
        {
            var s = string.Join(", ", parts);
            System.Diagnostics.Debug.WriteLine($"[BigDataCloud] {s}");
            return s;
        }
        return null;
    }
}