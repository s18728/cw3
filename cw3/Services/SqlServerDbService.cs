using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cw3.Services
{
    public class SqlServerDbService : IDbService
    {
        private s18728Context _dbContext;
        public SqlServerDbService([FromServices] DbContext dbContext)
        {
            this._dbContext = (s18728Context)dbContext;
        }

        // public IActionResult enrollStudent(Student student)
        // {
        //     Enrollment enrollment = new Enrollment();
        //
        //     using (var con = new SqlConnection(sqlCon))
        //     using (var com = new SqlCommand())
        //     {
        //         SqlTransaction sqlT = null;
        //         try
        //         {
        //             com.Connection = con;
        //             con.Open();
        //             sqlT = con.BeginTransaction();
        //             com.Transaction = sqlT;
        //             com.Parameters.AddWithValue("studiesName", student.Studies);
        //             var wynik = UseProcedure("checkIfExistsStudies", com);
        //             if (wynik.Count == 0) return new BadRequestResult();
        //
        //             com.CommandText = "SELECT 1 FROM Student WHERE Student.IndexNumber = @indexNumber";
        //             com.Parameters.AddWithValue("indexNumber", student.IndexNumber);
        //
        //             var dr = com.ExecuteReader();
        //             if (dr.Read()) return new BadRequestResult();
        //             dr.Close();
        //
        //             com.CommandText = "DECLARE @datetmp date = PARSE(@bdate as date USING 'en-GB');" +
        //                               " INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment, Password, Salt)" +
        //                               " VALUES (@indexNumber, @name, @lname, @datetmp, '1', @pass, @salt)";
        //             com.Parameters.Clear();
        //             com.Parameters.AddWithValue("indexNumber", student.IndexNumber);
        //
        //             com.Parameters.AddWithValue("name", student.FirstName);
        //             com.Parameters.AddWithValue("lname", student.LastName);
        //             com.Parameters.AddWithValue("bdate", student.BirthDate);
        //             com.ExecuteNonQuery();
        //
        //             com.Parameters.Clear();
        //             com.Parameters.AddWithValue("studiesName", student.Studies);
        //             com.Parameters.AddWithValue("indexNumber", student.IndexNumber);
        //             wynik = UseProcedure("enrollStudent", com);
        //
        //             enrollment.IdEnrollment = wynik[0][0];
        //             enrollment.IdStudy = wynik[0][2];
        //             enrollment.Semester = wynik[0][1];
        //             enrollment.StartDate = wynik[0][3];
        //
        //             sqlT.Commit();
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine(e);
        //             sqlT.Rollback();
        //             return new BadRequestResult();
        //         }
        //     }
        //
        //     ObjectResult objectResult = new ObjectResult(enrollment);
        //     objectResult.StatusCode = 201;
        //     return objectResult;
        // }

        public Enrollment promoteStudents(int studiesId, int semester)
        {
            try
            {
                var studiesGot = _dbContext.Studies.Single(s => s.IdStudy == studiesId);

                var oldEnrollment =
                    _dbContext.Enrollment.Single(e => e.IdStudy == studiesGot.IdStudy && e.Semester == semester);

                var newEnrollment =
                    _dbContext.Enrollment.SingleOrDefault(
                        e => e.IdStudy == studiesGot.IdStudy && e.Semester == semester + 1);

                if (newEnrollment == null)
                {
                    var newId = _dbContext.Enrollment.Max(e => e.IdEnrollment) + 1;
                    newEnrollment = new Enrollment
                    {
                        IdEnrollment = newId,
                        Semester = semester + 1,
                        IdStudy = studiesGot.IdStudy,
                        StartDate = DateTime.Now
                    };
                    _dbContext.Enrollment.Add(newEnrollment);
                    _dbContext.SaveChanges();
                }

                var studentsToUpdate = _dbContext.Student.Where(s => s.IdEnrollment == oldEnrollment.IdEnrollment).ToList();

                foreach (Student student in studentsToUpdate)
                {
                    student.IdEnrollment = newEnrollment.IdEnrollment;
                }

                _dbContext.SaveChanges();

                return newEnrollment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public string getStudentsString()
        {
            StringBuilder studentsSb = new StringBuilder();

            var students = _dbContext.Student.ToList();

            foreach (var student in students)
            {
                studentsSb.Append(student.IndexNumber).Append(" ").Append(student.FirstName).Append(" ")
                    .Append(student.LastName).AppendLine();
            }

            return studentsSb.ToString();
        }

        public bool modifyStudent(Student studentNewValue)
        {
            try
            {
                var student = _dbContext.Student.Single(s => s.IndexNumber.Equals(studentNewValue.IndexNumber));

                if (studentNewValue.FirstName != null)
                {
                    student.FirstName = studentNewValue.FirstName;
                }

                if (studentNewValue.LastName != null)
                {
                    student.LastName = studentNewValue.LastName;
                }

                if (studentNewValue.IdEnrollment != 0)
                {
                    student.IdEnrollment = studentNewValue.IdEnrollment;
                }

                if (studentNewValue.BirthDate != null)
                {
                    student.BirthDate = studentNewValue.BirthDate;
                }

                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
        }

        public bool removeStudent(string eska)
        {
            try
            {
                var student = _dbContext.Student.Single(s => s.IndexNumber.Equals(eska));
                _dbContext.Student.Remove(student);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
    }
}
