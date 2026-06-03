namespace FoodieApp.ViewModels;

public partial class RestaurantFinderViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;

    public RestaurantFinderViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        Title = "Nearby Restaurants";
    }

    [ObservableProperty]
    private ObservableCollection<Restaurant> _restaurants = new();

    [ObservableProperty]
    private bool _isLoadingLocation;

    [ObservableProperty]
    private string _locationStatus = "Searching for nearby restaurants...";

    [ObservableProperty]
    private double _currentLatitude;

    [ObservableProperty]
    private double _currentLongitude;

    [RelayCommand]
    private async Task FindNearbyRestaurantsAsync()
    {
        await ExecuteAsync(async () =>
        {
            IsLoadingLocation = true;
            LocationStatus = "Getting your location...";

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted
                    && (DeviceInfo.Current.Platform == DevicePlatform.iOS
                        || DeviceInfo.Current.Platform == DevicePlatform.Android))
                {
                    SetError(Constants.ErrorMessages.LocationDisabled);
                    return;
                }
                // On desktop, proceed anyway — system location service handles access
            }

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location == null)
            {
                SetError("Unable to get your location. Please try again.");
                return;
            }

            CurrentLatitude = location.Latitude;
            CurrentLongitude = location.Longitude;
            LocationStatus = $"Found {CurrentLatitude:F2}, {CurrentLongitude:F2}";

            var mockRestaurants = new List<Restaurant>
            {
                new() { Name = "The Italian Place", Cuisine = "Italian", Rating = 4.6, DistanceKm = 0.3, Latitude = CurrentLatitude + 0.001, Longitude = CurrentLongitude + 0.001, Address = "123 Main St", IsOpen = true },
                new() { Name = "Sushi Express", Cuisine = "Japanese", Rating = 4.4, DistanceKm = 0.5, Latitude = CurrentLatitude - 0.001, Longitude = CurrentLongitude + 0.002, Address = "456 Oak Ave", IsOpen = true },
                new() { Name = "Green Garden", Cuisine = "Vegetarian", Rating = 4.2, DistanceKm = 0.8, Latitude = CurrentLatitude + 0.002, Longitude = CurrentLongitude - 0.001, Address = "789 Elm Rd", IsOpen = false },
                new() { Name = "Spice Route", Cuisine = "Indian", Rating = 4.7, DistanceKm = 1.2, Latitude = CurrentLatitude - 0.003, Longitude = CurrentLongitude - 0.001, Address = "321 Pine St", IsOpen = true },
                new() { Name = "Burger Barn", Cuisine = "American", Rating = 4.0, DistanceKm = 1.5, Latitude = CurrentLatitude + 0.004, Longitude = CurrentLongitude - 0.002, Address = "654 Maple Dr", IsOpen = true },
                new() { Name = "Dragon Palace", Cuisine = "Chinese", Rating = 4.3, DistanceKm = 2.0, Latitude = CurrentLatitude - 0.004, Longitude = CurrentLongitude + 0.003, Address = "987 Cedar Ln", IsOpen = true }
            };

            Restaurants = new ObservableCollection<Restaurant>(
                mockRestaurants.OrderBy(r => r.DistanceKm));
        }, "Failed to find restaurants");

        IsLoadingLocation = false;
    }

    [RelayCommand]
    private async Task OpenRestaurantOnMapAsync(Restaurant restaurant)
    {
        if (restaurant == null) return;

        try
        {
            var mapLaunchOptions = new MapLaunchOptions
            {
                Name = restaurant.Name,
                NavigationMode = NavigationMode.Driving
            };

            await Map.Default.OpenAsync(
                new Location(restaurant.Latitude, restaurant.Longitude),
                mapLaunchOptions);

            if (!_settingsService.ReduceAnimations)
                HapticFeedbackHelper.PerformClick();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Map error: {ex.Message}");
            SetError("Unable to open maps. Please ensure a map app is installed.");
        }
    }

    [RelayCommand]
    private void ClearRestaurants()
    {
        Restaurants.Clear();
        LocationStatus = "Searching for nearby restaurants...";
        ClearError();
    }
}
