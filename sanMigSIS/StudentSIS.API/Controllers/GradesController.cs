namespace StudentSIS.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using StudentSIS.API.Models.DTOs;
using StudentSIS.Services;
using StudentSIS.Models;

[ApiController]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    private readonly DataService _dataService;

    public GradesController(DataService dataService)
    {
        _dataService = dataService;
    }

    
    
    
    
    [HttpGet]
    public ActionResult<List<Grade>> GetGrades(int studentId, string? term = null)
    {
        var student = _dataService.GetStudent(studentId);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        var grades = _dataService.GetStudentGrades(studentId, term);

        return Ok(grades);
    }

    
    
    
    
    [HttpPut]
    public ActionResult UpdateGrades([FromBody] List<Grade> updatedGrades)
    {
        if (updatedGrades == null || !updatedGrades.Any())
            return BadRequest(new { message = "Invalid grades data" });

        foreach (var update in updatedGrades)
        {
            var grade = _dataService.GetGrade(update.Id);
            if (grade != null)
            {
                grade.CourseCode = update.CourseCode;
                grade.CourseName = update.CourseName;
                grade.Term = update.Term;
                grade.Mark = update.Mark;
            }
        }

        return Ok(new { message = "Grades updated successfully" });
    }
}
