using System;
using Microsoft.AspNetCore.Mvc;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }


        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = _dbService.getStudentsString();

            return Ok(students);
        }

        [HttpPost]
        [Route("modify")]
        public IActionResult ModifyStudent([FromBody] Student student)
        {
            if (_dbService.modifyStudent(student))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpDelete]
        [Route("delete/{eska}")]
        public IActionResult DeleteStudent([FromRoute] string eska)
        {
            if (_dbService.removeStudent(eska))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
    
}