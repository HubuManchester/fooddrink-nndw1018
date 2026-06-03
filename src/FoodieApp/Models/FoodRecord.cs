namespace FoodieApp.Models;

/// <summary>
/// Represents a user-created food or drink record.
/// Contains nutritional data, optional photo, and optional GPS location where the item was consumed/purchased.
/// </summary>
public class FoodRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoFilePath { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationName { get; set; }
    public NutritionInfo Nutrition { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool HasPhoto => !string.IsNullOrWhiteSpace(PhotoFilePath);
    public bool HasLocation => Latitude.HasValue && Longitude.HasValue;
}
