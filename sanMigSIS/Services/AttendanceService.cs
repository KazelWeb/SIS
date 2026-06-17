namespace StudentSIS.Services;

using StudentSIS.Models;

/// <summary>
/// Attendance Service - Handles all attendance-related business logic.
/// Activity 3: Separation of Concerns - Attendance domain logic separated from data layer.
/// </summary>
public class AttendanceService
{
    private readonly IDataService dataService;

    public AttendanceService(IDataService dataService)
    {
        this.dataService = dataService;
    }

    /// <summary>
    /// Record attendance for a student in a course
    /// </summary>
    public void RecordAttendance(int studentId, int courseId, DateTime date, string status)
    {
        dataService.RecordAttendance(studentId, courseId, date, status);
    }

    /// <summary>
    /// Get attendance record for a student in a specific course
    /// Activity 2: LINQ filtering and calculations
    /// </summary>
    public AttendanceSummary GetCourseAttendanceSummary(int studentId, int courseId)
    {
        var records = dataService.GetStudentAttendance(studentId, courseId);

        if (records.Count == 0)
        {
            return new AttendanceSummary
            {
                StudentId = studentId,
                CourseId = courseId,
                TotalClasses = 0,
                AttendancePercentage = 0.0
            };
        }

        var presentCount = records.Count(a => a.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
        var percentage = Math.Round((double)presentCount / records.Count * 100, 2);

        return new AttendanceSummary
        {
            StudentId = studentId,
            CourseId = courseId,
            TotalClasses = records.Count,
            PresentDays = presentCount,
            AbsentDays = records.Count - presentCount,
            AttendancePercentage = percentage
        };
    }

    /// <summary>
    /// Get overall attendance for a student
    /// </summary>
    public OverallAttendanceSummary GetStudentOverallAttendance(int studentId)
    {
        var records = dataService.GetAllStudentAttendances(studentId);

        if (records.Count == 0)
        {
            return new OverallAttendanceSummary
            {
                StudentId = studentId,
                TotalAttendanceRecords = 0,
                OverallPercentage = 0.0
            };
        }

        var presentCount = records.Count(a => a.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
        var percentage = Math.Round((double)presentCount / records.Count * 100, 2);

        // Group by course
        var byCourse = records
            .GroupBy(a => a.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                CourseName = dataService.GetCourse(g.Key)?.Name ?? "Unknown",
                Count = g.Count(),
                Present = g.Count(a => a.Status.Equals("Present", StringComparison.OrdinalIgnoreCase))
            })
            .ToList();

        return new OverallAttendanceSummary
        {
            StudentId = studentId,
            TotalAttendanceRecords = records.Count,
            PresentDays = presentCount,
            AbsentDays = records.Count - presentCount,
            OverallPercentage = percentage,
            CourseBreakdown = byCourse.Select(c => new CourseAttendanceBreakdown
            {
                CourseName = c.CourseName,
                Percentage = Math.Round((double)c.Present / c.Count * 100, 2)
            }).ToList()
        };
    }

    /// <summary>
    /// Get students with low attendance
    /// </summary>
    public List<LowAttendanceAlert> GetLowAttendanceStudents(double threshold = 75.0)
    {
        var allStudents = dataService.GetAllStudents();

        return allStudents
            .Select(s => new { Student = s, Attendance = GetStudentOverallAttendance(s.Id) })
            .Where(x => x.Attendance.OverallPercentage < threshold && x.Attendance.TotalAttendanceRecords > 0)
            .Select(x => new LowAttendanceAlert
            {
                StudentName = $"{x.Student.FirstName} {x.Student.LastName}",
                AttendancePercentage = x.Attendance.OverallPercentage,
                TotalClasses = x.Attendance.TotalAttendanceRecords,
                RequiredAction = x.Attendance.OverallPercentage < 50 ? "Critical" : "Warning"
            })
            .OrderBy(a => a.AttendancePercentage)
            .ToList();
    }

    /// <summary>
    /// Bulk update attendance records
    /// </summary>
    public void BulkUpdateAttendance(List<Attendance> updates)
    {
        dataService.BulkUpdateAttendance(updates);
    }

    /// <summary>
    /// Generate attendance report for a course on a specific date
    /// Activity 2: LINQ grouping and aggregation
    /// </summary>
    public List<AttendanceReportEntry> GenerateAttendanceReport(int courseId, DateTime date)
    {
        var courseAttendance = dataService.GetAllAttendances()
            .Where(a => a.CourseId == courseId && a.Date.Date == date.Date)
            .ToList();

        return courseAttendance
            .Select(a =>
            {
                var student = dataService.GetStudent(a.StudentId);
                return new AttendanceReportEntry
                {
                    StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown",
                    StudentId = a.StudentId,
                    Status = a.Status,
                    Notes = a.Notes
                };
            })
            .OrderBy(e => e.StudentName)
            .ToList();
    }

    /// <summary>
    /// Get attendance statistics for the system
    /// </summary>
    public AttendanceStatistics GetSystemAttendanceStats()
    {
        var allRecords = dataService.GetAllAttendances();

        if (allRecords.Count == 0)
        {
            return new AttendanceStatistics();
        }

        var presentCount = allRecords.Count(a => a.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
        var absentCount = allRecords.Count(a => a.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase));

        return new AttendanceStatistics
        {
            TotalRecords = allRecords.Count,
            PresentCount = presentCount,
            AbsentCount = absentCount,
            OverallPercentage = Math.Round((double)presentCount / allRecords.Count * 100, 2),
            CoursesTracked = allRecords.Select(a => a.CourseId).Distinct().Count(),
            StudentsTracked = allRecords.Select(a => a.StudentId).Distinct().Count()
        };
    }
}

/// <summary>
/// DTO for course attendance summary
/// </summary>
public class AttendanceSummary
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int TotalClasses { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public double AttendancePercentage { get; set; }
}

/// <summary>
/// DTO for overall student attendance
/// </summary>
public class OverallAttendanceSummary
{
    public int StudentId { get; set; }
    public int TotalAttendanceRecords { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public double OverallPercentage { get; set; }
    public List<CourseAttendanceBreakdown> CourseBreakdown { get; set; } = new();
}

/// <summary>
/// DTO for course attendance breakdown
/// </summary>
public class CourseAttendanceBreakdown
{
    public string CourseName { get; set; } = string.Empty;
    public double Percentage { get; set; }
}

/// <summary>
/// DTO for low attendance alert
/// </summary>
public class LowAttendanceAlert
{
    public string StudentName { get; set; } = string.Empty;
    public double AttendancePercentage { get; set; }
    public int TotalClasses { get; set; }
    public string RequiredAction { get; set; } = string.Empty;
}

/// <summary>
/// DTO for attendance report entry
/// </summary>
public class AttendanceReportEntry
{
    public string StudentName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// DTO for system attendance statistics
/// </summary>
public class AttendanceStatistics
{
    public int TotalRecords { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double OverallPercentage { get; set; }
    public int CoursesTracked { get; set; }
    public int StudentsTracked { get; set; }
}
