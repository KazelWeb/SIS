namespace StudentSIS.Services;

using StudentSIS.Models;

/// <summary>
/// Course Service - Handles all course-related business logic.
/// Activity 3: Separation of Concerns - Course domain logic separated from data layer.
/// </summary>
public class CourseService
{
    private readonly IDataService dataService;

    public CourseService(IDataService dataService)
    {
        this.dataService = dataService;
    }

    /// <summary>
    /// Get detailed course information with enrollment stats
    /// </summary>
    public CourseDetailInfo? GetCourseDetails(int courseId)
    {
        var course = dataService.GetCourse(courseId);
        if (course == null) return null;

        var enrollments = dataService.GetAllEnrollments()
            .Where(e => e.CourseId == courseId).ToList();

        var grades = dataService.GetAllEnrollments()
            .Where(e => e.CourseId == courseId)
            .SelectMany(e => dataService.GetStudentGrades(e.StudentId))
            .Where(g => g.CourseId == courseId)
            .ToList();

        var averageGrade = grades.Count > 0 
            ? Math.Round(grades.Average(g => g.Mark ?? 0), 2)
            : 0.0;

        return new CourseDetailInfo
        {
            Course = course,
            EnrolledStudents = enrollments.Count,
            ActiveEnrollments = enrollments.Count(e => e.Status == "Active"),
            AverageGrade = averageGrade,
            GradesCount = grades.Count
        };
    }

    /// <summary>
    /// Get courses by instructor
    /// Activity 2: LINQ filtering
    /// </summary>
    public List<Course> GetCoursesByInstructor(string instructor)
    {
        return dataService.GetAllCourses()
            .Where(c => c.Instructor.Equals(instructor, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Filter courses by credits
    /// </summary>
    public List<Course> GetCoursesByCredits(int credits)
    {
        return dataService.GetAllCourses()
            .Where(c => c.Credits == credits)
            .ToList();
    }

    /// <summary>
    /// Add a new course with validation
    /// </summary>
    public bool AddCourse(string code, string name, string instructor, int credits)
    {
        // Validate course code uniqueness
        if (dataService.GetCourseByCode(code) != null)
        {
            return false; // Course code already exists
        }

        var newCourse = new Course
        {
            Code = code,
            Name = name,
            Instructor = instructor,
            Credits = credits
        };

        dataService.AddCourse(newCourse);
        return true;
    }

    /// <summary>
    /// Get students enrolled in a course
    /// </summary>
    public List<Student> GetEnrolledStudents(int courseId)
    {
        var enrollments = dataService.GetAllEnrollments()
            .Where(e => e.CourseId == courseId)
            .ToList();

        return enrollments
            .Select(e => dataService.GetStudent(e.StudentId))
            .Where(s => s != null)
            .Cast<Student>()
            .ToList();
    }

    /// <summary>
    /// Get course statistics
    /// </summary>
    public CourseStatistics GetCourseStatistics()
    {
        var allCourses = dataService.GetAllCourses();
        var totalCredits = allCourses.Sum(c => c.Credits);
        var avgCredits = allCourses.Count > 0 
            ? Math.Round((double)totalCredits / allCourses.Count, 2)
            : 0.0;

        return new CourseStatistics
        {
            TotalCourses = allCourses.Count,
            TotalCredits = totalCredits,
            AverageCredits = avgCredits,
            InstructorCount = allCourses.Select(c => c.Instructor).Distinct().Count()
        };
    }
}

/// <summary>
/// DTO for course detail information
/// </summary>
public class CourseDetailInfo
{
    public Course Course { get; set; } = new();
    public int EnrolledStudents { get; set; }
    public int ActiveEnrollments { get; set; }
    public double AverageGrade { get; set; }
    public int GradesCount { get; set; }
}

/// <summary>
/// DTO for course statistics
/// </summary>
public class CourseStatistics
{
    public int TotalCourses { get; set; }
    public int TotalCredits { get; set; }
    public double AverageCredits { get; set; }
    public int InstructorCount { get; set; }
}
