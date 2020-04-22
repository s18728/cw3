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
        public IActionResult enrollStudent(Student student);

        public IActionResult promoteStudents(StudiesInfo studies);

        public bool checkIfStudentExists(string indeks);

        public string getStudentsString();

        public Student getStudent(string indeks);

        public string getEnrollments(string id);

        public string setRefreshToken(string indeks, string token);

        public Student getStudentFromRefreshToken(string token);

        public void setSalt(string indeks, string salt);

        public string getSalt(string indeks);

        public void setPassword(string indeks, string pass);

        public void hashAllPasswords();

    }
}
