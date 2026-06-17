namespace StudentSIS.Services.Repositories;

/// <summary>
/// In-Memory Repository Implementation
/// Activity 4: Generic repository pattern for in-memory data storage.
/// Implements IRepository<T> for any entity type.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private List<T> data = new();

    public InMemoryRepository()
    {
    }

    /// <summary>
    /// Initialize repository with existing data
    /// </summary>
    public InMemoryRepository(List<T> initialData)
    {
        data = new List<T>(initialData);
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
        }
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    public void Delete(T entity)
    {
        if (entity != null)
        {
            data.Remove(entity);
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
    /// Save changes (no-op for in-memory)
    /// </summary>
    public void SaveChanges()
    {
        // In-memory data is already persistent during the session
    }

    /// <summary>
    /// Clear all data
    /// </summary>
    public void Clear()
    {
        data.Clear();
    }

    /// <summary>
    /// Get current count
    /// </summary>
    public int Count()
    {
        return data.Count;
    }
}
