using Microsoft.AspNetCore.Mvc;
using PORTAL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetMe()
        {
            var studentCode = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(studentCode))
            {
                return Unauthorized();
            }

            var student = 
                await _studentService.GetStudentbyStudentCode(studentCode);

            if (student == null)
            {
                return NotFound("Student not found");
            }
            return Ok(student);
        }
        
    }
}