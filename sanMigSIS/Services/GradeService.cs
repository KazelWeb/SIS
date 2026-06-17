namespace StudentSIS.Services;

using StudentSIS.Models;

/// <summary>
/// Grade Service - Handles all grade-related business logic and academic calculations.
/// Activity 3: Separation of Concerns - Grade domain logic separated from data layer.
/// </summary>
public class GradeService
{
    private readonly IDataService dataService;

    public GradeService(IDataService dataService)
    {
        this.dataService = dataService;
    }

    /// <summary>
    /// Record a grade for a student in a course
    /// </summary>
    public void RecordGrade(int studentId, int courseId, double mark)
    {
        var course = dataService.GetCourse(courseId);
        if (course == null) return;

        var existingGrade = dataService.GetStudentGrades(studentId)
            .FirstOrDefault(g => g.CourseId == courseId);

        if (existingGrade != null)
        {
            existingGrade.Mark = mark;
            // Letter grade is calculated dynamically via GetLetterGrade()
        }
        else
        {
            var newGrade = new Grade
            {
                StudentId = studentId,
                CourseId = courseId,
                CourseName = course.Name,
                CourseCode = course.Code,
                Term = DateTime.Now.Year.ToString(),
                Mark = mark
            };

            dataService.AddGrade(newGrade);
        }
    }

    /// <summary>
    /// Get academic summary for a student
    /// Activity 2: LINQ aggregation
    /// </summary>
    public AcademicSummary GetAcademicSummary(int studentId)
    {
        var grades = dataService.GetStudentGrades(studentId);

        if (grades.Count == 0)
        {
            return new AcademicSummary
            {
                StudentId = studentId,
                TotalCoursesTaken = 0,
                AverageMark = 0.0,
                GradeDistribution = new Dictionary<string, int>()
            };
        }

        var gradeDistribution = new Dictionary<string, int>
        {
            { "A", 0 },
            { "B", 0 },
            { "C", 0 },
            { "D", 0 },
            { "F", 0 }
        };

        foreach (var grade in grades)
        {
            var letterGrade = grade.GetLetterGrade();
            if (gradeDistribution.ContainsKey(letterGrade))
            {
                gradeDistribution[letterGrade]++;
            }
        }

        var markedGrades = grades.Where(g => g.Mark.HasValue).ToList();
        var averageMark = markedGrades.Count > 0 
            ? markedGrades.Average(g => g.Mark!.Value)
            : 0.0;

        return new AcademicSummary
        {
            StudentId = studentId,
            TotalCoursesTaken = grades.Count,
            AverageMark = Math.Round(averageMark, 2),
            GradeDistribution = gradeDistribution
        };
    }

    /// <summary>
    /// Get top performing students
    /// Activity 2: LINQ ordering and grouping
    /// </summary>
    public List<TopStudentInfo> GetTopPerformers(int count = 10)
    {
        var allStudents = dataService.GetAllStudents();

        return allStudents
            .Select(s => new TopStudentInfo
            {
                StudentId = s.Id,
                StudentName = $"{s.FirstName} {s.LastName}",
                AverageMark = CalculateStudentAverage(s.Id)
            })
            .OrderByDescending(ts => ts.AverageMark)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Get failing students
    /// </summary>
    public List<FailingStudentInfo> GetFailingStudents()
    {
        var allStudents = dataService.GetAllStudents();

        return allStudents
            .SelectMany(s => dataService.GetStudentGrades(s.Id)
                .Where(g => g.Mark.HasValue && g.Mark!.Value < 60)
                .Select(g => new FailingStudentInfo
                {
                    StudentId = s.Id,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    CourseName = g.CourseName,
                    Mark = g.Mark!.Value
                }))
            .ToList();
    }

    /// <summary>
    /// Calculate average mark for a student
    /// </summary>
    private double CalculateStudentAverage(int studentId)
    {
        var grades = dataService.GetStudentGrades(studentId);
        
        if (grades.Count == 0) return 0.0;

        var markedGrades = grades.Where(g => g.Mark.HasValue).ToList();
        
        if (markedGrades.Count == 0) return 0.0;

        return Math.Round(markedGrades.Average(g => g.Mark!.Value), 2);
    }

    /// <summary>
    /// Get grade statistics for the system
    /// </summary>
    public GradeStatistics GetGradeStatistics()
    {
        var allGrades = dataService.GetAllStudents()
            .SelectMany(s => dataService.GetStudentGrades(s.Id))
            .Where(g => g.Mark.HasValue)
            .ToList();

        if (allGrades.Count == 0)
        {
            return new GradeStatistics();
        }

        var marks = allGrades.Select(g => g.Mark!.Value).ToList();

        return new GradeStatistics
        {
            TotalGradesRecorded = allGrades.Count,
            AverageMark = Math.Round(marks.Average(), 2),
            HighestMark = marks.Max(),
            LowestMark = marks.Min(),
            PassingCount = allGrades.Count(g => g.Mark >= 60),
            FailingCount = allGrades.Count(g => g.Mark < 60)
        };
    }
}

/// <summary>
/// DTO for academic summary
/// </summary>
public class AcademicSummary
{
    public int StudentId { get; set; }
    public int TotalCoursesTaken { get; set; }
    public double AverageMark { get; set; }
    public Dictionary<string, int> GradeDistribution { get; set; } = new();
}

/// <summary>
/// DTO for top performing student
/// </summary>
public class TopStudentInfo
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public double AverageMark { get; set; }
}

/// <summary>
/// DTO for failing student
/// </summary>
public class FailingStudentInfo
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public double Mark { get; set; }
}

/// <summary>
/// DTO for grade statistics
/// </summary>
public class GradeStatistics
{
    public int TotalGradesRecorded { get; set; }
    public double AverageMark { get; set; }
    public double HighestMark { get; set; }
    public double LowestMark { get; set; }
    public int PassingCount { get; set; }
    public int FailingCount { get; set; }
}
