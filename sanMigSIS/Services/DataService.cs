namespace StudentSIS.Services;

using StudentSIS.Models;

/// <summary>
/// In-memory data service implementation.
/// Provides CRUD operations for all entities.
/// Activity 2: Implements IDataService interface using dependency injection.
/// </summary>
public class DataService : IDataService
{
    private List<Student> students = new();
    private List<Course> courses = new();
    private List<Grade> grades = new();
    private List<Enrollment> enrollments = new();
    private List<Attendance> attendances = new();

    private int studentId = 1;
    private int courseId = 1;
    private int gradeId = 1;
    private int enrollmentId = 1;
    private int attendanceId = 1;

    public DataService()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        
        students.Add(new Student
        {
            Id = studentId++,
            FirstName = "Juan",
            LastName = "Dela Cruz",
            Email = "juan.delacruz@example.com",
            PhoneNumber = "555-0101",
            Address = "123 Main St",
            DateOfBirth = new DateTime(2005, 3, 15)
        });

        students.Add(new Student
        {
            Id = studentId++,
            FirstName = "Maria",
            LastName = "Santos",
            Email = "maria.santos@example.com",
            PhoneNumber = "555-0102",
            Address = "456 Oak Ave",
            DateOfBirth = new DateTime(2004, 7, 22)
        });

        students.Add(new Student
        {
            Id = studentId++,
            FirstName = "Miguel",
            LastName = "Garcia",
            Email = "miguel.garcia@example.com",
            PhoneNumber = "555-0103",
            Address = "789 Pine Rd",
            DateOfBirth = new DateTime(2005, 1, 10)
        });

        
        courses.Add(new Course
        {
            Id = courseId++,
            Code = "CS101",
            Name = "Introduction to Computer Science",
            Instructor = "Dr. Santos",
            Credits = 3
        });

        courses.Add(new Course
        {
            Id = courseId++,
            Code = "MATH201",
            Name = "Calculus II",
            Instructor = "Prof. Cruz",
            Credits = 4
        });

        courses.Add(new Course
        {
            Id = courseId++,
            Code = "ENG102",
            Name = "English Composition",
            Instructor = "Dr. Reyes",
            Credits = 3
        });

        courses.Add(new Course
        {
            Id = courseId++,
            Code = "PHYS101",
            Name = "Physics I",
            Instructor = "Dr. Brown",
            Credits = 4
        });

        
        enrollments.Add(new Enrollment
        {
            Id = enrollmentId++,
            StudentId = 1,
            CourseId = 1,
            EnrollmentDate = new DateTime(2024, 1, 15),
            SemesterId = "FALL2024",
            Status = "Active"
        });

        enrollments.Add(new Enrollment
        {
            Id = enrollmentId++,
            StudentId = 1,
            CourseId = 2,
            EnrollmentDate = new DateTime(2024, 1, 15),
            SemesterId = "FALL2024",
            Status = "Active"
        });

        enrollments.Add(new Enrollment
        {
            Id = enrollmentId++,
            StudentId = 2,
            CourseId = 1,
            EnrollmentDate = new DateTime(2024, 1, 15),
            SemesterId = "FALL2024",
            Status = "Active"
        });

        enrollments.Add(new Enrollment
        {
            Id = enrollmentId++,
            StudentId = 2,
            CourseId = 3,
            EnrollmentDate = new DateTime(2024, 1, 15),
            SemesterId = "FALL2024",
            Status = "Active"
        });

        
        grades.Add(new Grade
        {
            Id = gradeId++,
            StudentId = 1,
            CourseId = 1,
            CourseName = "Introduction to Computer Science",
            CourseCode = "CS101",
            Term = "Fall2025",
            Mark = 92
        });

        grades.Add(new Grade
        {
            Id = gradeId++,
            StudentId = 1,
            CourseId = 2,
            CourseName = "Calculus II",
            CourseCode = "MATH201",
            Term = "Fall2025",
            Mark = 87
        });

        grades.Add(new Grade
        {
            Id = gradeId++,
            StudentId = 2,
            CourseId = 1,
            CourseName = "Introduction to Computer Science",
            CourseCode = "CS101",
            Term = "Fall2025",
            Mark = 78
        });

        grades.Add(new Grade
        {
            Id = gradeId++,
            StudentId = 2,
            CourseId = 3,
            CourseName = "English Composition",
            CourseCode = "ENG102",
            Term = "Fall2025",
            Mark = 95
        });

        
        attendances.Add(new Attendance
        {
            Id = attendanceId++,
            StudentId = 1,
            CourseId = 1,
            Date = DateTime.Today.AddDays(-5),
            Status = "Present"
        });

        attendances.Add(new Attendance
        {
            Id = attendanceId++,
            StudentId = 1,
            CourseId = 1,
            Date = DateTime.Today.AddDays(-3),
            Status = "Present"
        });

        attendances.Add(new Attendance
        {
            Id = attendanceId++,
            StudentId = 1,
            CourseId = 1,
            Date = DateTime.Today.AddDays(-1),
            Status = "Absent"
        });
    }

    
    public Student? GetStudent(int id)
    {
        return students.FirstOrDefault(s => s.Id == id);
    }

    public List<Student> GetAllStudents()
    {
        return students;
    }

    public void AddStudent(Student student)
    {
        student.Id = students.Count > 0 ? students.Max(s => s.Id) + 1 : 1;
        studentId = student.Id + 1;
        students.Add(student);
    }

    public void UpdateStudent(Student updatedStudent)
    {
        var existing = students.FirstOrDefault(s => s.Id == updatedStudent.Id);
        if (existing != null)
        {
            existing.FirstName = updatedStudent.FirstName;
            existing.LastName = updatedStudent.LastName;
            existing.Email = updatedStudent.Email;
            existing.PhoneNumber = updatedStudent.PhoneNumber;
            existing.Address = updatedStudent.Address;
            existing.DateOfBirth = updatedStudent.DateOfBirth;
        }
    }

    public void DeleteStudent(int id)
    {
        var student = students.FirstOrDefault(s => s.Id == id);
        if (student != null) students.Remove(student);
    }

    
    public Course? GetCourse(int id)
    {
        return courses.FirstOrDefault(c => c.Id == id);
    }

    public Course? GetCourseByCode(string courseCode)
    {
        return courses.FirstOrDefault(c => c.Code.Equals(courseCode, StringComparison.OrdinalIgnoreCase));
    }

    public List<Course> GetAllCourses()
    {
        return courses;
    }

    public void AddCourse(Course course)
    {
        course.Id = courseId++;
        courses.Add(course);
    }

    public void UpdateCourse(Course updatedCourse)
    {
        var existing = courses.FirstOrDefault(c => c.Id == updatedCourse.Id);
        if (existing != null)
        {
            existing.Code = updatedCourse.Code;
            existing.Name = updatedCourse.Name;
            existing.Instructor = updatedCourse.Instructor;
            existing.Credits = updatedCourse.Credits;
        }
    }

    public void DeleteCourse(int id)
    {
        var course = courses.FirstOrDefault(c => c.Id == id);
        if (course != null) courses.Remove(course);
    }

    
    public Grade? GetGrade(int id)
    {
        return grades.FirstOrDefault(g => g.Id == id);
    }

    public List<Grade> GetStudentGrades(int studentId, string? term = null)
    {
        var query = grades.Where(g => g.StudentId == studentId);
        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(g => g.Term.Equals(term.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    public void AddGrade(Grade grade)
    {
        grade.Id = gradeId++;
        grades.Add(grade);
    }

    
    public List<Enrollment> GetAllEnrollments()
    {
        return enrollments;
    }

    public List<Enrollment> GetStudentEnrollments(int studentId)
    {
        return enrollments.Where(e => e.StudentId == studentId).ToList();
    }

    public bool EnrollStudent(int studentId, int courseId)
    {
        return EnrollStudent(studentId, courseId, "FALL2026");
    }

    public bool EnrollStudent(int studentId, int courseId, string semesterId)
    {
        semesterId = string.IsNullOrWhiteSpace(semesterId) ? "FALL2026" : semesterId.Trim();

        if (enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
        {
            return false;
        }

        enrollments.Add(new Enrollment
        {
            Id = enrollmentId++,
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.Now,
            SemesterId = semesterId,
            Status = "Active"
        });

        var course = GetCourse(courseId);
        if (course != null)
        {
            grades.Add(new Grade
            {
                Id = gradeId++,
                StudentId = studentId,
                CourseId = courseId,
                CourseName = course.Name,
                CourseCode = course.Code,
                Term = semesterId,
                Mark = null
            });
        }

        return true;
    }

    
    public List<Attendance> GetAllAttendances()
    {
        return attendances;
    }

    public List<Attendance> GetStudentAttendance(int studentId, int courseId)
    {
        return attendances.Where(a => a.StudentId == studentId && a.CourseId == courseId).ToList();
    }

    public List<Attendance> GetAllStudentAttendances(int studentId)
    {
        return attendances.Where(a => a.StudentId == studentId).ToList();
    }

    public void RecordAttendance(int studentId, int courseId, DateTime date, string status)
    {
        
        var existing = attendances.FirstOrDefault(a => 
            a.StudentId == studentId && a.CourseId == courseId && a.Date.Date == date.Date);

        if (existing != null)
        {
            existing.Status = status;
        }
        else
        {
            attendances.Add(new Attendance
            {
                Id = attendanceId++,
                StudentId = studentId,
                CourseId = courseId,
                Date = date,
                Status = status
            });
        }
    }

    public void BulkUpdateAttendance(List<Attendance> updates)
    {
        foreach (var update in updates)
        {
            if (update.Id > 0)
            {
                var existing = attendances.FirstOrDefault(a => a.Id == update.Id);
                if (existing != null)
                {
                    existing.Date = update.Date;
                    existing.Status = update.Status;
                }
            }
            else
            {
                update.Id = attendanceId++;
                attendances.Add(update);
            }
        }
    }

    public bool DeleteAttendance(int studentId, int courseId, DateTime date)
    {
        var record = attendances.FirstOrDefault(a => 
            a.StudentId == studentId && a.CourseId == courseId && a.Date.Date == date.Date);

        if (record != null)
        {
            attendances.Remove(record);
            return true;
        }
        return false;
    }
}
