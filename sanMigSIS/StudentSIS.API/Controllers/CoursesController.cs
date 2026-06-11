
namespace StudentSIS.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using StudentSIS.Services;
using StudentSIS.Models;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly DataService _dataService;

    public CoursesController(DataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public ActionResult<List<Course>> GetAllCourses()
    {
        return Ok(_dataService.GetAllCourses());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Course> GetCourse(int id)
    {
        var course = _dataService.GetCourse(id);
        if (course == null) return NotFound(new { message = "Course not found" });
        return Ok(course);
    }

    [HttpPost]
    public ActionResult<Course> CreateCourse([FromBody] Course newCourse)
    {
        _dataService.AddCourse(newCourse);
        return CreatedAtAction(nameof(GetCourse), new { id = newCourse.Id }, newCourse);
    }

    [HttpPut("{id:int}")]
    public ActionResult UpdateCourse(int id, [FromBody] Course updatedCourse)
    {
        var course = _dataService.GetCourse(id);
        if (course == null) return NotFound(new { message = "Course not found" });
        
        updatedCourse.Id = id;
        _dataService.UpdateCourse(updatedCourse);
        return Ok(new { message = "Course updated successfully" });
    }

    [HttpDelete("{id:int}")]
    public ActionResult DeleteCourse(int id)
    {
        var course = _dataService.GetCourse(id);
        if (course == null) return NotFound(new { message = "Course not found" });
        
        _dataService.DeleteCourse(id);
        return NoContent();
    }
}