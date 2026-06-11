namespace StudentSIS.Models;

public class Grade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public double? Mark { get; set; }
    public string LetterGrade { get; set; } = string.Empty;

    public string GetLetterGrade()
    {
        if (!Mark.HasValue) return "-";
        return Mark.Value switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }
}
