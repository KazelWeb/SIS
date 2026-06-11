namespace StudentSIS.API.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class AttendanceUpdateDto
{
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    public int CourseId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;
}
