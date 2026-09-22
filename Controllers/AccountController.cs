using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PORTAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PORTAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount(
            string studentCode,
            string password)
        {
            var user = new ApplicationUser
            {
                UserName = studentCode,
                StudentCode = studentCode
            };

            var result = await _userManager.CreateAsync(
                user,
                password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                Message = "Account created successfully",
                StudentCode = studentCode
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string studentCode, String password)
        {
            var user = await _userManager.FindByNameAsync(studentCode);

            if (user == null)
            {
                return Unauthorized("Invalid Student Code or Password");
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if(!result)
            {
                return Unauthorized("Invalid Student Code or Password");

            }
            var claims = new[]
              {
        new Claim(
            ClaimTypes.Name,
            user.StudentCode)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "PORTAL_SECRET_KEY_CHANGE_THIS_LATER"));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "PORTAL",
                audience: "PORTAL",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);
            return Ok(new
            {
                message = "Login Successful",
                studentCode = user.StudentCode,
                Token = tokenString

            });
        }
    }
}