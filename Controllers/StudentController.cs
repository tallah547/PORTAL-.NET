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


        [Authorize]
        [HttpGet("available-units")]
        public async Task<IActionResult> GetAvailableUnits()
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

            var units = await _studentService.GetAvailableUnits(student.Programme_Code, student.Year_of_Study_Semester);

            return Ok(units);


        }

        [Authorize]
        [HttpPost("register-units")]
        public async Task<IActionResult> RegisterUnits([FromBody] RegisterUnitsRequest request)
        {
            var studentCode = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(studentCode))
            {
                return Unauthorized();
            }

            if (request == null || request.SelectedUnits == null || request.SelectedUnits.Count == 0)
            {
                return BadRequest("No units selected.");
            }

            var student = await _studentService.GetStudentbyStudentCode(studentCode);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var registration = new CreateCourseRegistration
            {
                Student_Code = student.Code,
                First_Name = student.First_Name,
                Middle_Name = student.Middle_Name,
                SurName = student.SurName,
                E_Mail = student.E_Mail,
                Phone_No = student.Phone_No,
                Gender = student.Gender,
                Residency = request.Residency,
                Portal_User_Id = student.Portal_User_Id,
                Created_By = "",
                Department_Code = student.Department_Code,
                Department_Name = student.Department_Name,
                Programme_Code = student.Programme_Code,
                Programme_Name = student.Programme_Name,
                Year_of_Study_Semester = student.Year_of_Study_Semester,
                Posted = false
            };

            var existingRegistrations = await _studentService.GetCourseRegistrations(student.Code, student.Year_of_Study_Semester);

            var existingRegistration = existingRegistrations.FirstOrDefault();

            if (existingRegistration != null)
            {
                return BadRequest(
                    "You have already registered for units.");
            }

            var result = await _studentService.CreateCourseRegistration(registration);

            if (result == null)
            {
                return BadRequest("Course Registration could not be created");
            }
            var availableUnits = await _studentService.GetAvailableUnits(student.Programme_Code, student.Year_of_Study_Semester);

            foreach (var courseCode in request.SelectedUnits)
            {

                var unit = availableUnits.FirstOrDefault(u => u.Course_Code == courseCode);

                if (unit == null)
                {
                    return BadRequest($"Unit {courseCode} could not be found.");
                }

                var line = new CourseRegistrationLineCreate
                {
                    Programme_Code = student.Programme_Code,
                    Student_Code = student.Code,
                    Year_of_Study = student.Year_of_Study_Semester,
                    Registration_Code = result.Code,
                    Entry_No = request.SelectedUnits.IndexOf(courseCode),
                    Course_Code = unit.Course_Code,
                    Description = unit.Description,
                    Lecturer = unit.Lecturer,
                    Name = unit.Name,
                    Hours = unit.Hours
                };
                await _studentService.CreateCourseRegistrationLine(line);

            }
            // Call BC Codeunit through SOAP
            await _studentService.RegisterUnits(student.Code, student.Year_of_Study_Semester, student.Programme_Code);

            return Ok(result);
        }
    }
}