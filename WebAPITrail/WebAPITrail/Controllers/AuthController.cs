using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPITrail.Models;
using WebAPITrail.Services;

namespace WebAPITrail.Controllers
{
    [Route("api/[controller]")]
    
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]

        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            
            var result = await _authService.RegisterAsync(model);

            var stu = await _context.Students.FindAsync(int.Parse(model.username));

            if (stu == null)
            {
                return BadRequest($"No student with the student ID {model.username}");
            }


            if (!result.IsAuthenticated )
            {
                return BadRequest(result.Message);
            }
            return Ok(result);

        }

        [HttpPost("login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);


            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleAsync ([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest($"{result}");
            }


            return Ok(result);
        }
    }

  

}


    

