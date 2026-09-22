using Microsoft.AspNetCore.Mvc;
using PORTAL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PORTAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController: ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var result = await _studentService.GetStudents();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var studentCode = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                studentCode = studentCode
            });
        }
        
    }
}