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

    }
}
