using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebAPITrail.dtos;

namespace WebAPITrail.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : ControllerBase
    {

        public readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.courses.ToListAsync();

            return Ok(courses);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(string id) {

            var course = await _context.courses.FindAsync(id);
            if (course == null) {
                return NotFound($"no course was found with id : {id}");

            }

            return Ok(course);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseDto dto)
        {
            var Ecourse = await _context.courses.FindAsync(dto.courseId);

             if (Ecourse != null)
             {
                 return BadRequest("Course already assigned");
             }
            var course = new Course() { courseId = dto.courseId, courseName = dto.courseName, departments = dto.departments };

            _context.courses.AddAsync(course);
            _context.SaveChanges();
            return Ok(course);

        }

        [Authorize(Roles = "Admin")]
        [HttpPut]

        public async Task<IActionResult> ModifyCourseInfo(CreateCourseDto dto)
        {
            var course =  await _context.courses.FindAsync(dto.courseId);
            if (course == null)
            {
                return NotFound($"no course was found with id :{dto.courseId}");
            }
            course.courseName = dto.courseName;
            course.departments = dto.departments;

            _context.SaveChanges();
            return Ok(course);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            var course =  await _context.courses.FindAsync(id);
            if (course is null)
            {
                return NotFound($"no course was found with id :{id}");
            }
            _context.courses.Remove(course);
             _context.SaveChanges();
            return Ok(course);



        }

    }
}
