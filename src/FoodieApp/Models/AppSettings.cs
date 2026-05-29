namespace FoodieApp.Models;

public class AppSettings
{
    public bool IsDarkMode { get; set; }
    public double FontScale { get; set; } = 1.0;
    public bool ReduceAnimations { get; set; }
    public bool HighContrast { get; set; }
    public string Language { get; set; } = "en";
}
