using System;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    }
    
}