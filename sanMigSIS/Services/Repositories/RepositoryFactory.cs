namespace StudentSIS.Services.Repositories;

/// <summary>
/// Repository Factory
/// Activity 4: Factory pattern to switch between different data access strategies.
/// Demonstrates how to use InMemory, JsonData, or Database repositories.
/// </summary>
public class RepositoryFactory
{
    public enum RepositoryType
    {
        InMemory,
        JsonData,
        Database
    }

    private readonly string dataDirectory;

    public RepositoryFactory(string dataDirectory = "")
    {
        this.dataDirectory = string.IsNullOrEmpty(dataDirectory) 
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")
            : dataDirectory;
    }

    /// <summary>
    /// Create a repository instance based on type
    /// </summary>
    public IRepository<T> CreateRepository<T>(RepositoryType type, string? entityName = null) where T : class
    {
        var name = entityName ?? typeof(T).Name;

        return type switch
        {
            RepositoryType.InMemory => new InMemoryRepository<T>(),
            RepositoryType.JsonData => new JsonDataRepository<T>(dataDirectory, name),
            RepositoryType.Database => new DatabaseRepository<T>(),
            _ => throw new ArgumentException($"Unknown repository type: {type}")
        };
    }

    /// <summary>
    /// Create multiple repositories at once
    /// </summary>
    public Dictionary<string, object> CreateRepositories<T>(RepositoryType type) where T : class
    {
        return new Dictionary<string, object>
        {
            { typeof(T).Name, CreateRepository<T>(type) }
        };
    }
}
