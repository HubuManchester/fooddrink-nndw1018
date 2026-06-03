namespace FoodieApp.Helpers;

public static class HapticFeedbackHelper
{
    public static void PerformClick()
    {
        Perform(HapticFeedbackType.Click);
    }

    public static void PerformLongPress()
    {
        Perform(HapticFeedbackType.LongPress);
    }

    private static void Perform(HapticFeedbackType type)
    {
        try
        {
            HapticFeedback.Default.Perform(type);
        }
        catch (FeatureNotSupportedException)
        {
            // Haptic feedback not supported on this device
            System.Diagnostics.Debug.WriteLine("Haptic feedback not supported on this device.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Haptic error: {ex.Message}");
        }
    }
}
