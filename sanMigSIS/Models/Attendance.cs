namespace StudentSIS.Models;

public class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Present"; 
    public string Notes { get; set; } = string.Empty;
}
