namespace StudentSIS.Models;

using System.ComponentModel.DataAnnotations;

public class Student
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "The FirstName field is required.")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The LastName field is required.")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The Email field is required.")]
    [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateOfBirth { get; set; }
}
