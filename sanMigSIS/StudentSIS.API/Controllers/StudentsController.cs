namespace StudentSIS.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using StudentSIS.API.Models.DTOs;
using StudentSIS.Services;
using StudentSIS.Models;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly DataService _dataService;

    public StudentsController(DataService dataService)
    {
        _dataService = dataService;
    }

    
    
    
    
    [HttpGet]
    public ActionResult<List<Student>> GetAllStudents()
    {
        return Ok(_dataService.GetAllStudents());
    }

    
    
    
    
    [HttpGet("{id:int}")]
    public ActionResult<Student> GetStudent(int id)
    {
        var student = _dataService.GetStudent(id);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        return Ok(student);
    }

    
    
    
    
    [HttpGet("grades")]
    public ActionResult<List<Grade>> GetStudentGrades(int studentId, string? term = null)
    {
        var student = _dataService.GetStudent(studentId);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        var grades = _dataService.GetStudentGrades(studentId, term);

        return Ok(grades);
    }

    [HttpPut("grades")]
    public ActionResult UpdateStudentGrades([FromQuery] int? studentId, [FromBody] List<Grade> updatedGrades)
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

    
    
    
    
    [HttpPut("{id:int}")]
    public ActionResult UpdateStudent(int id, [FromBody] Student updatedStudent)
    {
        var student = _dataService.GetStudent(id);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        updatedStudent.Id = id;
        _dataService.UpdateStudent(updatedStudent);

        return Ok(new { message = "Student updated successfully" });
    }

    
    
    
    
    [HttpPost]
    public ActionResult<Student> CreateStudent([FromBody] Student newStudent)
    {
        _dataService.AddStudent(newStudent);
        return CreatedAtAction(nameof(GetStudent), new { id = newStudent.Id }, newStudent);
    }

    
    
    
    
    [HttpDelete("{id:int}")]
    public ActionResult DeleteStudent(int id)
    {
        var student = _dataService.GetStudent(id);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        _dataService.DeleteStudent(id);
        return NoContent();
    }
}
