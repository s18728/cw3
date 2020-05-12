using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        //TODO to change
        public IActionResult enrollStudent(Student student);

        //TODO also this
        public IActionResult promoteStudents(StudiesInfo studies);

        public bool checkIfStudentExists(string indeks);

        public string getStudentsString();

        public Student getStudent(string indeks);

        public string getEnrollments(string id);

    }
}
