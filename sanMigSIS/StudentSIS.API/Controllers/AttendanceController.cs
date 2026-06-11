namespace StudentSIS.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using StudentSIS.API.Models.DTOs;
using StudentSIS.Services;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly DataService _dataService;

    public AttendanceController(DataService dataService)
    {
        _dataService = dataService;
    }

    
    
    
    
    [HttpPut]
    public ActionResult UpdateAttendance([FromBody] System.Text.Json.JsonElement payload)
    {
        if (payload.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            var updates = System.Text.Json.JsonSerializer.Deserialize<List<StudentSIS.Models.Attendance>>(
                payload.GetRawText(), 
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (updates == null || !updates.Any())
                return BadRequest(new { message = "Invalid attendance data" });

            _dataService.BulkUpdateAttendance(updates);
            return Ok(new { message = "Attendance records updated successfully" });
        }
        else if (payload.ValueKind == System.Text.Json.JsonValueKind.Object)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<AttendanceUpdateDto>(
                payload.GetRawText(), 
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null || string.IsNullOrWhiteSpace(request.Status))
                return BadRequest(new { message = "Invalid attendance update request" });

            var student = _dataService.GetStudent(request.StudentId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            var course = _dataService.GetCourse(request.CourseId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            _dataService.RecordAttendance(request.StudentId, request.CourseId, request.Date, request.Status);

            var response = new AttendanceResponseDto
            {
                StudentId = request.StudentId,
                UpdatedAt = DateTime.UtcNow,
                Status = "Recorded"
            };

            return Ok(response);
        }

        return BadRequest(new { message = "Invalid payload" });
    }

    
    
    
    
    [HttpGet]
    public ActionResult<List<StudentSIS.Models.Attendance>> GetAllAttendances()
    {
        var records = _dataService.GetAllAttendances();
        return Ok(records);
    }   

    
    
    
    
    [HttpGet("{studentId}")]
    public ActionResult<List<StudentSIS.Models.Attendance>> GetStudentAttendances(int studentId)
    {
        var student = _dataService.GetStudent(studentId);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        var records = _dataService.GetAllStudentAttendances(studentId);
        return Ok(records);
    }

    
    
    
    
    [HttpGet("course/{courseId}")]
    public ActionResult<List<StudentSIS.Models.Attendance>> GetCourseAttendances(int courseId)
    {
        var course = _dataService.GetCourse(courseId);
        if (course == null)
            return NotFound(new { message = "Course not found" });

        var records = _dataService.GetAllAttendances().Where(a => a.CourseId == courseId).ToList();
        return Ok(records);
    }

    
    
    
    
    [HttpDelete]
    public ActionResult DeleteAttendance([FromQuery] int studentId, [FromQuery] int courseId, [FromQuery] string date)
    {
        // Revert URL-decoded space back to '+' for ISO 8601 timezone parsing
        if (!DateTime.TryParse(date.Replace(" ", "+"), out DateTime parsedDate))
            return BadRequest(new { message = "Invalid date format." });

        var student = _dataService.GetStudent(studentId);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        bool deleted = _dataService.DeleteAttendance(studentId, courseId, parsedDate);
        if (!deleted)
            return NotFound(new { message = "Attendance record not found" });

        return NoContent();
    }

    
    
    
    
    // Bulk endpoint merged into PUT /api/Attendance
}
