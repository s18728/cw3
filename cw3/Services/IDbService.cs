﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public interface IDbService
    {
        //TODO to change
        // public IActionResult enrollStudent(Student student);

        public Enrollment promoteStudents(int studiesId, int semester);

        public string getStudentsString();

        public bool modifyStudent(Student studentNewValue);

        public bool removeStudent(string eska);

    }
}