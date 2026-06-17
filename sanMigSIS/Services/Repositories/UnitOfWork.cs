namespace StudentSIS.Services.Repositories;

using StudentSIS.Models;

/// <summary>
/// Unit of Work Pattern Implementation
/// Activity 4: Coordinates multiple repositories for complete data access layer.
/// Demonstrates how different repository implementations work together.
/// </summary>
public class UnitOfWork : IDisposable
{
    private readonly RepositoryFactory.RepositoryType repositoryType;
    private readonly RepositoryFactory factory;

    private IRepository<Student>? studentRepository;
    private IRepository<Course>? courseRepository;
    private IRepository<Grade>? gradeRepository;
    private IRepository<Enrollment>? enrollmentRepository;
    private IRepository<Attendance>? attendanceRepository;

    public UnitOfWork(RepositoryFactory.RepositoryType type = RepositoryFactory.RepositoryType.InMemory)
    {
        repositoryType = type;
        factory = new RepositoryFactory();
    }

    /// <summary>
    /// Student repository
    /// </summary>
    public IRepository<Student> Students 
        => studentRepository ??= factory.CreateRepository<Student>(repositoryType, "Students");

    /// <summary>
    /// Course repository
    /// </summary>
    public IRepository<Course> Courses 
        => courseRepository ??= factory.CreateRepository<Course>(repositoryType, "Courses");

    /// <summary>
    /// Grade repository
    /// </summary>
    public IRepository<Grade> Grades 
        => gradeRepository ??= factory.CreateRepository<Grade>(repositoryType, "Grades");

    /// <summary>
    /// Enrollment repository
    /// </summary>
    public IRepository<Enrollment> Enrollments 
        => enrollmentRepository ??= factory.CreateRepository<Enrollment>(repositoryType, "Enrollments");

    /// <summary>
    /// Attendance repository
    /// </summary>
    public IRepository<Attendance> Attendances 
        => attendanceRepository ??= factory.CreateRepository<Attendance>(repositoryType, "Attendances");

    /// <summary>
    /// Save all changes
    /// </summary>
    public void SaveChanges()
    {
        Students?.SaveChanges();
        Courses?.SaveChanges();
        Grades?.SaveChanges();
        Enrollments?.SaveChanges();
        Attendances?.SaveChanges();
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        SaveChanges();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Example usage of UnitOfWork with different repository types
/// </summary>
public static class UnitOfWorkExamples
{
    /// <summary>
    /// Example: Using InMemory repository
    /// </summary>
    public static void UseInMemoryRepository()
    {
        using (var unitOfWork = new UnitOfWork(RepositoryFactory.RepositoryType.InMemory))
        {
            var student = new Student
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DateOfBirth = DateTime.Now.AddYears(-20)
            };

            unitOfWork.Students.Add(student);
            unitOfWork.SaveChanges();

            var retrieved = unitOfWork.Students.GetById(1);
            // Data persists only during application runtime
        }
    }

    /// <summary>
    /// Example: Using JSON Data repository
    /// </summary>
    public static void UseJsonDataRepository()
    {
        using (var unitOfWork = new UnitOfWork(RepositoryFactory.RepositoryType.JsonData))
        {
            var student = new Student
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                DateOfBirth = DateTime.Now.AddYears(-21)
            };

            unitOfWork.Students.Add(student);
            unitOfWork.SaveChanges();

            // Data is persisted to JSON files on disk
            // Survives application restarts
        }
    }

    /// <summary>
    /// Example: Using Database repository (placeholder)
    /// </summary>
    public static void UseDatabaseRepository()
    {
        using (var unitOfWork = new UnitOfWork(RepositoryFactory.RepositoryType.Database))
        {
            // Requires database configuration
            // Would be implemented with Entity Framework Core
            /*
            var student = new Student { ... };
            unitOfWork.Students.Add(student);
            unitOfWork.SaveChanges();
            */
        }
    }
}
