using cw3.DTOs;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        [HttpPost]
        [Route("enroll")]
        public IActionResult EnrollStudent([FromBody]EnrollStudentRequestDTO request, [FromServices]IDbService dbService)
        {
            Student studentToEnroll = new Student
            {
                IndexNumber = request.IndexNumber,
                LastName = request.LastName,
                FirstName = request.FirstName,
                BirthDate = request.BirthDate
            };

            Enrollment tmp = dbService.enrollStudent(studentToEnroll, request.StudyName);
            if (tmp == null) return BadRequest();

            EnrollStudentResponseDTO response = new EnrollStudentResponseDTO
            {
                Semester = tmp.Semester,
                IdStudy = tmp.IdStudy,
                StartDate = tmp.StartDate,
                IdEnrollment = tmp.IdEnrollment
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("promotions")]
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