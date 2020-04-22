using System;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cw3.DTOs;
using cw3.Security;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private IStudentsDbService _dbService;
        private IConfiguration _configuration;

        public StudentsController(IStudentsDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;
        }


        [HttpGet]
        [Authorize(Roles = "student,employee")]
        public IActionResult GetStudents()
        {
            //_dbService.hashAllPasswords();
            var students = _dbService.getStudentsString();

            return Ok(students);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "student,employee")]
        public IActionResult GetStudentEnrollments(string id)
        {
            var enrollments = _dbService.getEnrollments(id);
            return Ok(enrollments);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginRequestDTO loginRequest)
        {
            if (loginRequest.Login.Equals("admin") && loginRequest.Password.Equals("admin"))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loginRequest.Login),
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "student"),
                    new Claim(ClaimTypes.Role, "employee") 
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "JakubSpZoo",
                    audience: "students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: cred
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = _dbService.setRefreshToken("admin", Guid.NewGuid().ToString())
                });
            }
            else
            {
                var ifStudentExists = _dbService.checkIfStudentExists(loginRequest.Login);
                if (!ifStudentExists) return Unauthorized();

                var student = _dbService.getStudent(loginRequest.Login);

                var salt = _dbService.getSalt(student.IndexNumber);
                
                if (!Salter.Validate(loginRequest.Password, salt, student.Password)) return Unauthorized();

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loginRequest.Login),
                    new Claim(ClaimTypes.Name, student.FirstName + " " + student.LastName),
                    new Claim(ClaimTypes.Role, "student")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "JakubSpZoo",
                    audience: "students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: cred
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = _dbService.setRefreshToken(student.IndexNumber,Guid.NewGuid().ToString())
                });
            }
        }


        [HttpPost]
        [Route("ref-token/{refToken}")]
        public IActionResult RefreshToken(string refToken)
        {
            var stud = _dbService.getStudentFromRefreshToken(refToken);
            if (stud == null) return NotFound();

            if (stud.IndexNumber.Equals("admin"))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "admin"),
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "student"),
                    new Claim(ClaimTypes.Role, "employee")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "JakubSpZoo",
                    audience: "students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: cred
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = _dbService.setRefreshToken("admin", Guid.NewGuid().ToString())
                });
            }
            else
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, stud.IndexNumber),
                    new Claim(ClaimTypes.Name, stud.FirstName + " " + stud.LastName),
                    new Claim(ClaimTypes.Role, "student")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "JakubSpZoo",
                    audience: "students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: cred
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = _dbService.setRefreshToken(stud.IndexNumber, Guid.NewGuid().ToString())
                });
            }
        }
    }
}