using Microsoft.Maui.Controls;

namespace FoodieApp.Helpers;

public static class AccessibilityHelper
{
    public static void ConfigureAccessibleElement(VisualElement element, string description, string hint = "")
    {
        if (element == null) return;

        SemanticProperties.SetDescription(element, description);
        if (!string.IsNullOrEmpty(hint))
        {
            SemanticProperties.SetHint(element, hint);
        }
    }

    public static double GetScaledFontSize(double baseSize, double scale)
    {
        double scaledSize = baseSize * scale;
        return Math.Clamp(scaledSize, baseSize * Constants.MinFontScale, baseSize * Constants.MaxFontScale);
    }

    public static bool HasHighContrastSupport()
    {
        return Application.Current?.RequestedTheme == AppTheme.Dark;
    }

    public static Color GetHighContrastColor(Color lightColor, Color darkColor, bool isDarkMode)
    {
        return isDarkMode ? darkColor : lightColor;
    }
}
