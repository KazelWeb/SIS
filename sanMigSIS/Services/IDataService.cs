namespace StudentSIS.Services;

using StudentSIS.Models;

/// <summary>
/// Interface for data service operations.
/// Implements abstraction principle for data access.
/// </summary>
public interface IDataService
{
    #region Student Operations
    Student? GetStudent(int id);
    List<Student> GetAllStudents();
    void AddStudent(Student student);
    void UpdateStudent(Student updatedStudent);
    void DeleteStudent(int id);
    #endregion

    #region Course Operations
    Course? GetCourse(int id);
    Course? GetCourseByCode(string courseCode);
    List<Course> GetAllCourses();
    void AddCourse(Course course);
    void UpdateCourse(Course updatedCourse);
    void DeleteCourse(int id);
    #endregion

    #region Grade Operations
    Grade? GetGrade(int id);
    List<Grade> GetStudentGrades(int studentId, string? term = null);
    void AddGrade(Grade grade);
    #endregion

    #region Enrollment Operations
    List<Enrollment> GetAllEnrollments();
    List<Enrollment> GetStudentEnrollments(int studentId);
    bool EnrollStudent(int studentId, int courseId);
    bool EnrollStudent(int studentId, int courseId, string semesterId);
    #endregion

    #region Attendance Operations
    List<Attendance> GetAllAttendances();
    List<Attendance> GetStudentAttendance(int studentId, int courseId);
    List<Attendance> GetAllStudentAttendances(int studentId);
    void RecordAttendance(int studentId, int courseId, DateTime date, string status);
    void BulkUpdateAttendance(List<Attendance> updates);
    bool DeleteAttendance(int studentId, int courseId, DateTime date);
    #endregion
}
