using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [Authorize(Roles = "employee")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        //TODO change to DTOs and return type
        [HttpPost]
        public IActionResult EnrollStudent([FromBody]Student student, [FromServices]IStudentsDbService dbService)
        {
            if (student.FirstName == null || student.LastName == null || student.IndexNumber == null
                || student.BirthDate == null || student.Studies == null) return BadRequest();

            return dbService.enrollStudent(student);
        }

        
        //TODO DTOs and return type
        [HttpPost("promotions")]
        public IActionResult PromoteSemester([FromBody] StudiesInfo studies, [FromServices] IStudentsDbService dbService)
        {
            return dbService.promoteStudents(studies);
        }


    }
}