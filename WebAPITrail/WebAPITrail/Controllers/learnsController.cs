using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Xml;
using WebAPITrail.dtos;

namespace WebAPITrail.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class learnsController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public learnsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var learning =  await _context.Learns.ToListAsync();
            return Ok(learning);
        }
        [HttpGet("courses/{studentId}")]
        public async Task<ActionResult<IEnumerable<AssignStudentToClassDto>>> GetCoursesForStudent(int studentId)
        {
            var courses = await _context.Learns
                .Where(l => l.StudentId == studentId)
                .Include(l => l.course) 
                .Select(l => new AssignStudentToClassDto
                {
                    StudentId = l.StudentId,
                    courseId = l.courseId,
                    courseName = l.course.courseName, 
                    year = l.year,
                    semester = l.semester,
                    grade = l.grade
                })
                .ToListAsync();

            return Ok(courses);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("students/{courseId}")]
        public async Task<ActionResult<IEnumerable<AssignStudentToClassDto>>> GetStudentsForCourse(string courseId)
        {
            var students = await _context.Learns
                .Where(l => l.courseId == courseId)
                .Include(l => l.student) // Include the related student data
                .Select(l => new CreatStudentDto
                {
                    StudentId = l.StudentId,
                    StudentName=l.student.StudentName,
                    level = l.student.level
                           
                })
                .ToListAsync();

            return Ok(students);
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AssignStudentToCourse(AssignStudentToClassDto dto)
        {
            var learning = await _context.Learns.FindAsync(dto.StudentId, dto.courseId, dto.semester, dto.year);
            if (learning != null)
            {
                return BadRequest($"student {dto.StudentId} is already assigned to course {dto.courseId}");
            }
            var learns = new Learns() { courseId = dto.courseId, StudentId = dto.StudentId, semester = dto.semester, year = dto.year };

            _context.Learns.AddAsync(learns);
            _context.SaveChanges();

            return Ok(learns);

        }
        [HttpPut]
        [Authorize (Roles ="Admin")]
        public async Task<IActionResult> GradeStudent(AssignStudentToClassDto dto)
        {
            var learning = await _context.Learns.FindAsync(dto.StudentId, dto.courseId, dto.semester, dto.year);
            if(learning == null || dto.grade == null)
            {
                return NotFound($"there is no record of this student {dto.StudentId} being registered to this course {dto.courseId}");
            }
            learning.grade = dto.grade;
            _context.SaveChanges();

            return Ok(learning);

        }
        [Authorize(Roles = "User")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRecord(AssignStudentToClassDto dto)
        {
            var course = await _context.courses.FindAsync(dto.courseId);
            if (course is null)
            {
                return NotFound($"no course was found with id :{dto.courseId}");
            }
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
            {
                return NotFound($"no student was found with this id :{dto.StudentId}");
            }

            var learning = await _context.Learns.FindAsync(dto.StudentId, dto.courseId, dto.semester, dto.year);
            if (learning == null)
            {
                return NotFound($"there is no record of this student {dto.StudentId} being registered to this course {dto.courseId}");
            }
            _context.Learns.Remove(learning);
            _context.SaveChanges();
            return Ok(learning);
        }


    }
}
