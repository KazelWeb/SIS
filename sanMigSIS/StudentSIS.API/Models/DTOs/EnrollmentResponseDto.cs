namespace StudentSIS.API.Models.DTOs;

public class EnrollmentResponseDto
{
    public string Status { get; set; } = string.Empty;
    public int EnrollmentId { get; set; }
    public string Message { get; set; } = string.Empty;
}
