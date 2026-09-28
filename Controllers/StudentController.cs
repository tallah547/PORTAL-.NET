using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PORTAL.Models;
using PORTAL.Services;
using System.Security.Claims;

namespace PORTAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
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

            var student = await _studentService.GetStudentbyStudentCode(studentCode);

            if (student == null)
            {
                return NotFound("Student not found");
            }
            return Ok(student);
        }

        [Authorize]
        [HttpGet("fees")]
        public async Task<IActionResult> GetFees()
        {
            var studentCode = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(studentCode))
            {
                return Unauthorized();
            }

            var student = await _studentService.GetStudentbyStudentCode(studentCode);

            if (student == null)
            {
                return NotFound("Student Not Found");
            }

            var summary = await _studentService.GetStudentFeeSummary(student.Customer_No);

            return Ok(summary);
        }


        [Authorize]
        [HttpPut("phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] UpdatePhoneRequest request)
        {
            var studentCode = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(studentCode))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest("Phone number is required.");
            }

            var student = await _studentService.GetStudentbyStudentCode(studentCode);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var updated = await _studentService.UpdateStudentPhone(studentCode, request.PhoneNumber);

            if (!updated)
            {
                return BadRequest("Phone number could not be updated.");
            }

            return Ok(new
            {
                message = "Phone number updated successfully."
            });
        }

        [Authorize]
        [HttpGet("course-lines")]
        public async Task<IActionResult> GetCourseLines()
        {
            var studentCode = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(studentCode))
            {
                return Unauthorized();
            }

            var student = await _studentService.GetStudentbyStudentCode(studentCode);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var registrations = await _studentService.GetCourseRegistrations(student.Code, student.Year_of_Study_Semester);

            if (registrations == null || registrations.Count == 0)
            {
                return NotFound("Course Registration not found");
            }

            var registration = registrations.FirstOrDefault(r => r.Posted);

            if (registration == null)
            {
                return NotFound("Posted course registration not found");
            }

            var result = await _studentService.GetCourseRegistrationLines(registration.Code);

            return Ok(result);
        }


    }
}