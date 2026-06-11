namespace StudentSIS.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using StudentSIS.API.Models.DTOs;
using StudentSIS.Services;
using StudentSIS.Models;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly DataService _dataService;

    public EnrollmentsController(DataService dataService)
    {
        _dataService = dataService;
    }

    
    
    
    
    [HttpPost]
    public ActionResult<EnrollmentResponseDto> EnrollStudentInCourse([FromBody] EnrollmentRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.CourseCode))
            return BadRequest(new { message = "Invalid enrollment request" });

        var student = _dataService.GetStudent(request.StudentId);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        var course = _dataService.GetCourseByCode(request.CourseCode);
        if (course == null)
            return NotFound(new { message = "Course not found" });

        bool enrolled = _dataService.EnrollStudent(request.StudentId, course.Id, request.SemesterId);
        
        if (!enrolled)
            return BadRequest(new { message = "Student already enrolled in this course" });

        
        var enrollments = _dataService.GetStudentEnrollments(request.StudentId);
        var newEnrollment = enrollments.LastOrDefault();

        var response = new EnrollmentResponseDto
        {
            Status = "Success",
            EnrollmentId = newEnrollment?.Id ?? 0,
            Message = $"Student successfully enrolled in {course.Name}."
        };

        return Ok(response);
    }
}
