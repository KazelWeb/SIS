namespace StudentSIS.API.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class EnrollmentRequestDto
{
    [Required(ErrorMessage = "The StudentId field is required.")]
    public int StudentId { get; set; }
    
    [Required(ErrorMessage = "The CourseCode field is required.")]
    public string CourseCode { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The SemesterId field is required.")]
    public string SemesterId { get; set; } = string.Empty;
}
