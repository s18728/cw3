using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // [HttpGet]
        // public string GetStudent()
        // {
        //     return "Kowalski, Malewski, Andrzejewski";
        // }

        // [HttpGet("{id}")]
        // public IActionResult GetStudent(int id)
        // {
        //     if (id == 1)
        //     {
        //         return Ok("Kowalski");
        //     }else if (id == 2)
        //     {
        //         return Ok("Malewski");
        //     }
        //
        //     return NotFound("Nie znaleziono studenta!");
        // }

        // private readonly IDbService _dbService;
        //
        // public StudentsController(IDbService dbService)
        // {
        //     _dbService = dbService;
        // }

        // private static List<Student> students = new List<Student>();

        [HttpGet]
        public IActionResult GetStudent()
        {
            List<Student> students = new List<Student>();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "SELECT FirstName, LastName, BirthDate, Semester, Name FROM Student " +
                    "INNER JOIN Enrollment ON Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "INNER JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy";

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.Semester = dr["Semester"].ToString();
                    st.NameOfStudies = dr["Name"].ToString();
                    students.Add(st);
                }

            }

            return Ok(students);
        }

        // [HttpPost]
        // public IActionResult CreateStudent(Student student)
        // {
        //     student.IndexNumber = $"s{new Random().Next(1, 20000)}";
        //     students.Add(student);
        //     return Ok(student);
        // }

        // [HttpPut]
        // public IActionResult UpdateStudent([FromQuery]int id, [FromBody] Student student)
        // {
        //     if (id>=students.Count)
        //     {
        //         return NotFound();
        //     }
        //
        //     students.RemoveAt(id);
        //     students.Insert(id, student);
        //     return Ok("Aktualizacja ukonczona");
        // }
        
        // [HttpDelete]
        // public IActionResult DeleteStudent([FromQuery] int id)
        // {
        //     if (id >= students.Count)
        //     {
        //         return NotFound();
        //     }
        //
        //     students.RemoveAt(id);
        //     return Ok("Usunieto studenta o id " + id);
        // }
    }
}