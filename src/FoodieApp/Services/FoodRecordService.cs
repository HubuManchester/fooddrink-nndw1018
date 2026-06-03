namespace FoodieApp.Services;

public class FoodRecordService : IFoodRecordService
{
    private readonly List<FoodRecord> _records;
    private int _nextId = 1;
    private readonly object _lock = new();

    public FoodRecordService()
    {
        _records = new List<FoodRecord>();
    }

    public Task<int> SaveFoodRecordAsync(FoodRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        try
        {
            lock (_lock)
            {
                if (record.Id <= 0)
                {
                    record.Id = _nextId++;
                    record.CreatedAt = DateTime.Now;
                    _records.Add(record);
                }
                else
                {
                    var existing = _records.FirstOrDefault(r => r.Id == record.Id);
                    if (existing != null)
                    {
                        var index = _records.IndexOf(existing);
                        _records[index] = record;
                    }
                    else
                    {
                        _records.Add(record);
                    }
                }
            }

            return Task.FromResult(record.Id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving food record: {ex.Message}");
            throw;
        }
    }

    public Task<List<FoodRecord>> GetAllFoodRecordsAsync()
    {
        try
        {
            lock (_lock)
            {
                var result = _records
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
                return Task.FromResult(result);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting food records: {ex.Message}");
            return Task.FromResult(new List<FoodRecord>());
        }
    }

    public Task<FoodRecord?> GetFoodRecordByIdAsync(int id)
    {
        try
        {
            lock (_lock)
            {
                var record = _records.FirstOrDefault(r => r.Id == id);
                return Task.FromResult(record);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting food record {id}: {ex.Message}");
            return Task.FromResult<FoodRecord?>(null);
        }
    }

    public Task<bool> DeleteFoodRecordAsync(int id)
    {
        try
        {
            lock (_lock)
            {
                var removed = _records.RemoveAll(r => r.Id == id);
                return Task.FromResult(removed > 0);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting food record {id}: {ex.Message}");
            return Task.FromResult(false);
        }
    }
}
