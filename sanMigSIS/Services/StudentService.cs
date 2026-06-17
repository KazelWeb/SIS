namespace StudentSIS.Services;

using StudentSIS.Models;

/// <summary>
/// Student Service - Handles all student-related business logic.
/// Activity 3: Separation of Concerns - Student domain logic separated from data layer.
/// </summary>
public class StudentService
{
    private readonly IDataService dataService;

    public StudentService(IDataService dataService)
    {
        this.dataService = dataService;
    }

    /// <summary>
    /// Get student with their enrolled courses and current GPA
    /// </summary>
    public StudentProfileInfo? GetStudentProfile(int studentId)
    {
        var student = dataService.GetStudent(studentId);
        if (student == null) return null;

        var enrollments = dataService.GetStudentEnrollments(studentId);
        var grades = dataService.GetStudentGrades(studentId);
        var attendance = dataService.GetAllStudentAttendances(studentId);

        var gpa = CalculateGPA(grades);
        var attendancePercentage = CalculateAttendancePercentage(attendance);

        return new StudentProfileInfo
        {
            Student = student,
            EnrolledCourses = enrollments.Count,
            CurrentGPA = gpa,
            AttendancePercentage = attendancePercentage,
            TotalClasses = attendance.Count
        };
    }

    /// <summary>
    /// Calculate student's GPA from grades
    /// </summary>
    public double CalculateGPA(List<Grade> grades)
    {
        if (grades.Count == 0) return 0.0;

        // Simple GPA calculation: A=4.0, B=3.0, C=2.0, D=1.0, F=0.0
        var totalPoints = grades.Sum(g =>
        {
            var letterGrade = g.GetLetterGrade();
            return letterGrade switch
            {
                "A" => 4.0,
                "B" => 3.0,
                "C" => 2.0,
                "D" => 1.0,
                _ => 0.0
            };
        });

        return Math.Round(totalPoints / grades.Count, 2);
    }

    /// <summary>
    /// Calculate attendance percentage
    /// </summary>
    public double CalculateAttendancePercentage(List<Attendance> records)
    {
        if (records.Count == 0) return 0.0;

        var presentCount = records.Count(a => a.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
        return Math.Round((double)presentCount / records.Count * 100, 2);
    }

    /// <summary>
    /// Register a new student
    /// </summary>
    public void RegisterStudent(string firstName, string lastName, string email, 
        string phoneNumber, string address, DateTime dateOfBirth)
    {
        var newStudent = new Student
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address,
            DateOfBirth = dateOfBirth
        };

        dataService.AddStudent(newStudent);
    }

    /// <summary>
    /// Search students by name (using LINQ - Activity 2 C# concept)
    /// </summary>
    public List<Student> SearchStudents(string searchTerm)
    {
        var allStudents = dataService.GetAllStudents();
        
        // Activity 2: LINQ expression for filtering
        return allStudents
            .Where(s => s.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       s.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Get student statistics
    /// </summary>
    public StudentStatistics GetStudentStatistics()
    {
        var allStudents = dataService.GetAllStudents();
        var allGrades = allStudents.SelectMany(s => dataService.GetStudentGrades(s.Id)).ToList();

        return new StudentStatistics
        {
            TotalStudents = allStudents.Count,
            AverageGPA = allGrades.Count > 0 
                ? Math.Round(allGrades.Average(g => double.Parse(g.GetLetterGrade() switch
                {
                    "A" => "4.0",
                    "B" => "3.0",
                    "C" => "2.0",
                    "D" => "1.0",
                    _ => "0.0"
                })), 2)
                : 0.0
        };
    }
}

/// <summary>
/// DTO for student profile information
/// </summary>
public class StudentProfileInfo
{
    public Student Student { get; set; } = new();
    public int EnrolledCourses { get; set; }
    public double CurrentGPA { get; set; }
    public double AttendancePercentage { get; set; }
    public int TotalClasses { get; set; }
}

/// <summary>
/// DTO for student statistics
/// </summary>
public class StudentStatistics
{
    public int TotalStudents { get; set; }
    public double AverageGPA { get; set; }
}
