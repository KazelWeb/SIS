namespace StudentSIS.Services.Repositories;

using System.Text.Json;

/// <summary>
/// JSON Data Repository Implementation
/// Activity 4: Generic repository pattern with JSON file persistence.
/// Stores entities in JSON files for data durability.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class JsonDataRepository<T> : IRepository<T> where T : class
{
    private List<T> data = new();
    private readonly string filePath;
    private readonly JsonSerializerOptions jsonOptions;

    public JsonDataRepository(string dataDirectory, string entityName)
    {
        // Create data directory if it doesn't exist
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        filePath = Path.Combine(dataDirectory, $"{entityName}.json");
        jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        LoadFromFile();
    }

    /// <summary>
    /// Load data from JSON file
    /// </summary>
    private void LoadFromFile()
    {
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                data = JsonSerializer.Deserialize<List<T>>(json, jsonOptions) ?? new List<T>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading JSON data: {ex.Message}");
            data = new List<T>();
        }
    }

    /// <summary>
    /// Save data to JSON file
    /// </summary>
    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(data, jsonOptions);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving JSON data: {ex.Message}");
        }
    }

    /// <summary>
    /// Get entity by ID
    /// </summary>
    public T? GetById(int id)
    {
        return data.FirstOrDefault(item =>
        {
            var idProperty = item?.GetType().GetProperty("Id");
            return idProperty?.GetValue(item)?.Equals(id) ?? false;
        });
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public IEnumerable<T> GetAll()
    {
        return data.AsReadOnly();
    }

    /// <summary>
    /// Find entities matching predicate
    /// Activity 2: LINQ predicate filtering
    /// </summary>
    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        return data.Where(predicate).ToList();
    }

    /// <summary>
    /// Add entity
    /// </summary>
    public void Add(T entity)
    {
        if (entity != null)
        {
            data.Add(entity);
            SaveToFile();
        }
    }

    /// <summary>
    /// Update entity
    /// </summary>
    public void Update(T entity)
    {
        if (entity == null) return;

        var idProperty = entity.GetType().GetProperty("Id");
        if (idProperty == null) return;

        var entityId = idProperty.GetValue(entity);
        var existing = data.FirstOrDefault(item =>
        {
            var itemIdProperty = item?.GetType().GetProperty("Id");
            return itemIdProperty?.GetValue(item)?.Equals(entityId) ?? false;
        });

        if (existing != null)
        {
            var index = data.IndexOf(existing);
            data[index] = entity;
            SaveToFile();
        }
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    public void Delete(T entity)
    {
        if (entity != null && data.Remove(entity))
        {
            SaveToFile();
        }
    }

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    public void DeleteById(int id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            Delete(entity);
        }
    }

    /// <summary>
    /// Save changes (already saved on each operation)
    /// </summary>
    public void SaveChanges()
    {
        SaveToFile();
    }

    /// <summary>
    /// Get current count
    /// </summary>
    public int Count()
    {
        return data.Count;
    }

    /// <summary>
    /// Clear all data
    /// </summary>
    public void Clear()
    {
        data.Clear();
        SaveToFile();
    }
}
