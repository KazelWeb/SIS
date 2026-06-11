namespace StudentSIS.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string SemesterId { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; 
}
