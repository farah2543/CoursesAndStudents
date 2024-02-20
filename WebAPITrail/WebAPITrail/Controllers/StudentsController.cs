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
    public class StudentsController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        private new List<string> _allwoedExtentions = new List<string>() { ".png", ".jpg" };
        private  const long _maxAllowedSize = 1048576;


        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

                                                                                                                                                                                 
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var students = await _context.Students.ToListAsync();

            return Ok(students);


        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentAsync(int id)
        {
            var student =  await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound($"no student was found with this id :{id}");

            }
            return Ok(student);


        }
        [HttpGet("{id}/photo")]
        public async Task<IActionResult> GetStudentPhotoAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound($"No student was found with this id: {id}");
            }

            if (student.photo != null) // Assuming the photo is stored as a byte array in the database
            {
                return File(student.photo, "image/jpeg");
            }

            return NotFound("No photo found for this student.");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromForm]CreatStudentDto dto)
        {
            if(!_allwoedExtentions.Contains(Path.GetExtension(dto.photo.FileName.ToUpper()))) {
                return NotFound("only .png or .jpg extensions are allowed!");
            
            }

            if(dto.photo.Length > _maxAllowedSize)
            {
                return NotFound("photo too big the Max allowed size is 1 MB");
  

            }
            var find =  _context.Students.Find(dto.StudentId);
            if (find != null)
            {
                return BadRequest($"Student Already registered :{dto.StudentId}");

            }
            using var dataStream = new MemoryStream();
            await dto.photo.CopyToAsync(dataStream);

            var student = new Student() {
                photo = dataStream.ToArray(),
                StudentId = dto.StudentId,
                StudentName = dto.StudentName,
                level = dto.level
            };
           

            _context.Students.Add(student);
            _context.SaveChanges();
            return Ok(student);

        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> ModifyStudentInfo(CreatStudentDto dto)
        {
            var student =  await _context.Students.FindAsync(dto.StudentId);
            if(student is null) { 
                return NotFound($"no student was found with this id :{dto.StudentId}");
            
            }

            student.StudentName = dto.StudentName;
            student.level = dto.level;

             _context.SaveChanges();
            return Ok(student);

        }
        [HttpDelete("{id}")]

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound($"no student was found with this id :{id}");
            }
            _context.Students.Remove(student);
            _context.SaveChanges();
            return Ok(student);

            

        }

 
    }
}
