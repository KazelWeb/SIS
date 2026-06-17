namespace StudentSIS.Services;

using StudentSIS.Models;

/// <summary>
/// Enrollment Service - Handles all enrollment-related business logic.
/// Activity 3: Separation of Concerns - Enrollment domain logic separated from data layer.
/// </summary>
public class EnrollmentService
{
    private readonly IDataService dataService;

    public EnrollmentService(IDataService dataService)
    {
        this.dataService = dataService;
    }

    /// <summary>
    /// Enroll a student in a course with validation
    /// </summary>
    public EnrollmentResult EnrollStudent(int studentId, int courseId, string semesterId = "")
    {
        var student = dataService.GetStudent(studentId);
        if (student == null)
        {
            return new EnrollmentResult { Success = false, Message = "Student not found" };
        }

        var course = dataService.GetCourse(courseId);
        if (course == null)
        {
            return new EnrollmentResult { Success = false, Message = "Course not found" };
        }

        var success = dataService.EnrollStudent(studentId, courseId, semesterId);
        
        return new EnrollmentResult
        {
            Success = success,
            Message = success 
                ? $"Successfully enrolled {student.FirstName} {student.LastName} in {course.Name}"
                : "Student is already enrolled in this course"
        };
    }

    /// <summary>
    /// Get student's course schedule
    /// Activity 2: LINQ with joins
    /// </summary>
    public List<CourseSchedule> GetStudentSchedule(int studentId)
    {
        var enrollments = dataService.GetStudentEnrollments(studentId);

        return enrollments
            .Select(e => 
            {
                var course = dataService.GetCourse(e.CourseId);
                return new CourseSchedule
                {
                    CourseCode = course?.Code ?? "N/A",
                    CourseName = course?.Name ?? "Unknown",
                    Instructor = course?.Instructor ?? "N/A",
                    Credits = course?.Credits ?? 0,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    SemesterId = e.SemesterId
                };
            })
            .OrderBy(cs => cs.SemesterId)
            .ToList();
    }

    /// <summary>
    /// Get enrollment statistics for a course
    /// </summary>
    public EnrollmentStats GetCourseEnrollmentStats(int courseId)
    {
        var course = dataService.GetCourse(courseId);
        if (course == null)
        {
            return new EnrollmentStats { CourseId = courseId };
        }

        var enrollments = dataService.GetAllEnrollments()
            .Where(e => e.CourseId == courseId)
            .ToList();

        var semesters = enrollments
            .Select(e => e.SemesterId)
            .Distinct()
            .ToList();

        return new EnrollmentStats
        {
            CourseId = courseId,
            CourseName = course.Name,
            TotalEnrollments = enrollments.Count,
            ActiveEnrollments = enrollments.Count(e => e.Status == "Active"),
            InactiveEnrollments = enrollments.Count(e => e.Status != "Active"),
            UniqueSemesters = semesters.Count,
            Semesters = semesters
        };
    }

    /// <summary>
    /// Get all enrollments for current semester
    /// Activity 2: LINQ filtering
    /// </summary>
    public List<EnrollmentInfo> GetCurrentSemesterEnrollments(string semesterId)
    {
        return dataService.GetAllEnrollments()
            .Where(e => e.SemesterId.Equals(semesterId, StringComparison.OrdinalIgnoreCase))
            .Select(e =>
            {
                var student = dataService.GetStudent(e.StudentId);
                var course = dataService.GetCourse(e.CourseId);
                
                return new EnrollmentInfo
                {
                    StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown",
                    CourseName = course?.Name ?? "Unknown",
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                };
            })
            .ToList();
    }

    /// <summary>
    /// Check if student can enroll (credit limit validation)
    /// </summary>
    public bool CanEnroll(int studentId, int courseId)
    {
        var student = dataService.GetStudent(studentId);
        var course = dataService.GetCourse(courseId);

        if (student == null || course == null)
            return false;

        // Check if already enrolled
        var existing = dataService.GetStudentEnrollments(studentId)
            .FirstOrDefault(e => e.CourseId == courseId);

        if (existing != null)
            return false;

        // Check credit limit (max 18 credits per semester)
        var currentEnrollments = dataService.GetStudentEnrollments(studentId);
        var totalCredits = currentEnrollments
            .Sum(e => dataService.GetCourse(e.CourseId)?.Credits ?? 0);

        return (totalCredits + course.Credits) <= 18;
    }

    /// <summary>
    /// Get enrollment statistics for all courses
    /// </summary>
    public SystemEnrollmentStats GetSystemEnrollmentStats()
    {
        var allEnrollments = dataService.GetAllEnrollments();
        var allStudents = dataService.GetAllStudents();
        var allCourses = dataService.GetAllCourses();

        var enrolledStudents = allEnrollments
            .Select(e => e.StudentId)
            .Distinct()
            .Count();

        var enrolledCourses = allEnrollments
            .Select(e => e.CourseId)
            .Distinct()
            .Count();

        return new SystemEnrollmentStats
        {
            TotalEnrollments = allEnrollments.Count,
            ActiveStudents = enrolledStudents,
            CoursesWithEnrollments = enrolledCourses,
            TotalStudents = allStudents.Count,
            TotalCourses = allCourses.Count,
            EnrollmentRate = allStudents.Count > 0 
                ? Math.Round((double)enrolledStudents / allStudents.Count * 100, 2)
                : 0.0
        };
    }
}

/// <summary>
/// Result of an enrollment operation
/// </summary>
public class EnrollmentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// DTO for course schedule
/// </summary>
public class CourseSchedule
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int Credits { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SemesterId { get; set; } = string.Empty;
}

/// <summary>
/// DTO for enrollment statistics
/// </summary>
public class EnrollmentStats
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public int InactiveEnrollments { get; set; }
    public int UniqueSemesters { get; set; }
    public List<string> Semesters { get; set; } = new();
}

/// <summary>
/// DTO for enrollment info
/// </summary>
public class EnrollmentInfo
{
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO for system-wide enrollment statistics
/// </summary>
public class SystemEnrollmentStats
{
    public int TotalEnrollments { get; set; }
    public int ActiveStudents { get; set; }
    public int CoursesWithEnrollments { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public double EnrollmentRate { get; set; }
}
