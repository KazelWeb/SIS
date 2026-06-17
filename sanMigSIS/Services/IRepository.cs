namespace StudentSIS.Services;

/// <summary>
/// Generic repository interface for data access operations.
/// Implements Repository Pattern for Activity 4.
/// Supports multiple implementations: InMemory, JsonData, Database
/// </summary>
/// <typeparam name="T">Entity type (Student, Course, Grade, etc.)</typeparam>
public interface IRepository<T> where T : class
{
    // Read operations
    T? GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Func<T, bool> predicate);

    // Write operations
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void DeleteById(int id);

    // Persistence
    void SaveChanges();
}
