namespace StudentSIS.API.Models.DTOs;

public class GradeDto
{
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public int Credits { get; set; }
}
