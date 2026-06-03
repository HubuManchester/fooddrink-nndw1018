namespace FoodieApp.Services;

/// <summary>
/// Service for managing user-created food and drink records.
/// Supports CRUD operations with optional photo and location data.
/// </summary>
public interface IFoodRecordService
{
    /// <summary>
    /// Saves a food record. If the record has Id 0, a new Id is assigned and the record is created.
    /// Otherwise, the existing record is updated.
    /// </summary>
    /// <returns>The Id of the saved record.</returns>
    Task<int> SaveFoodRecordAsync(FoodRecord record);

    /// <summary>
    /// Returns all food records ordered by creation date (newest first).
    /// </summary>
    Task<List<FoodRecord>> GetAllFoodRecordsAsync();

    /// <summary>
    /// Looks up a single food record by its unique Id.
    /// Returns null if not found.
    /// </summary>
    Task<FoodRecord?> GetFoodRecordByIdAsync(int id);

    /// <summary>
    /// Deletes a food record by Id. Also cleans up any associated photo file.
    /// Returns true if the record was found and deleted.
    /// </summary>
    Task<bool> DeleteFoodRecordAsync(int id);
}
