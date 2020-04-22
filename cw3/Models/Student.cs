using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3
{
    public class Student
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string BirthDate { get; set; }

        public string Semester { get; set; }

        public string Studies { get; set; }

        public string IndexNumber { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return FirstName + "   " + LastName + "   " + Studies + "   " + Semester;
        }
    }
}
