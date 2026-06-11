namespace StudentSIS.API.Models.DTOs;

public class AttendanceResponseDto
{
    public int StudentId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
