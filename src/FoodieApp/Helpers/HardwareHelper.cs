namespace FoodieApp.Helpers;

/// <summary>
/// Provides hardware capability detection and user-friendly fallback messages
/// for features that depend on device hardware (camera, GPS, TTS, haptics).
/// </summary>
public static class HardwareHelper
{
    /// <summary>
    /// Returns true if the device is a mobile platform (iOS or Android).
    /// Desktop platforms may lack camera, GPS, and other mobile hardware.
    /// </summary>
    public static bool IsMobilePlatform =>
        DeviceInfo.Current.Platform == DevicePlatform.iOS ||
        DeviceInfo.Current.Platform == DevicePlatform.Android;

    /// <summary>
    /// Gets a user-friendly message explaining why a hardware feature is unavailable.
    /// </summary>
    /// <param name="feature">One of: camera, gps, location, tts, speech, haptic, vibration</param>
    /// <returns>A localized-friendly message string.</returns>
    public static string GetUnavailableMessage(string feature)
    {
        return feature.ToLowerInvariant() switch
        {
            "camera" => "Camera is not available on this device. You can still fill in the form without a photo.",
            "gps" or "location" => "Location services are not available on this device. You can still save your record without a location.",
            "tts" or "speech" => "Text-to-speech is not supported on this device.",
            "haptic" or "vibration" => "Haptic feedback is not supported on this device.",
            _ => $"The requested feature ({feature}) is not available on this device."
        };
    }

    /// <summary>
    /// Checks if the camera permission is denied and returns an appropriate message.
    /// On desktop (Windows/macOS), camera access is controlled by system privacy
    /// settings rather than per-app runtime permissions — let it proceed.
    /// </summary>
    public static async Task<string?> CheckCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                // On desktop, permission prompt may not show; let the actual
                // API call handle the failure with a better error message.
                if (IsMobilePlatform)
                    return "Camera permission is required to take photos. Please enable it in your device Settings.";
                // Fall through — let MediaPicker try anyway
            }
        }

        // Let the actual hardware API handle availability — desktop webcams work fine.
        return null;
    }

    /// <summary>
    /// Checks if location permission is denied and returns an appropriate message.
    /// On desktop (Windows/macOS), location uses system location services
    /// rather than per-app runtime permissions — let it proceed.
    /// </summary>
    public static async Task<string?> CheckLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                if (IsMobilePlatform)
                    return "Location permission is required to record your position. Please enable it in your device Settings.";
                // Fall through — let Geolocation API try anyway (Windows uses system location service)
            }
        }

        // Let the actual hardware API handle availability — desktop location services work.
        return null;
    }
}
