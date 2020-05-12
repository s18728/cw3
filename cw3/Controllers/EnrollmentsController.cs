using cw3.DTOs;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        // //TODO change to DTOs and return type
        // [HttpPost]
        // public IActionResult EnrollStudent([FromBody]Student student, [FromServices]IDbService dbService)
        // {
        //     if (student.FirstName == null || student.LastName == null || student.IndexNumber == null
        //         || student.BirthDate == null || student.Studies == null) return BadRequest();
        //
        //     return dbService.enrollStudent(student);
        // }

        [HttpPost("promotions")]
        public IActionResult PromoteSemester([FromBody] PromoteRequestDTO request, [FromServices] IDbService dbService)
        { 
            var newEnrollment = dbService.promoteStudents(request.StudiesId, request.Semester);

            if (newEnrollment == null)
            {
                return BadRequest();
            }
            else
            {
                var response = new PromoteResponseDTO
                {
                    Semester = newEnrollment.Semester,
                    IdStudy = newEnrollment.IdStudy,
                    StartDate = newEnrollment.StartDate,
                    IdEnrollment = newEnrollment.IdEnrollment
                };
                return Ok(response);
            }
        }


    }
}