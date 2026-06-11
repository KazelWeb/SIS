namespace StudentSIS.Models;

using System.ComponentModel.DataAnnotations;

public class Course
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "The Code field is required.")]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The Name field is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The Instructor field is required.")]
    [StringLength(100)]
    public string Instructor { get; set; } = string.Empty;
    
    [Range(1, 6)]
    public int Credits { get; set; }
}
