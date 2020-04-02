using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private string sqlCon = "Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True";

        public IActionResult enrollStudent(Student student)
        {
            Enrollment enrollment = new Enrollment();

            using (var con = new SqlConnection(sqlCon))
            using (var com = new SqlCommand())
            {
                SqlTransaction sqlT = null;
                try
                {
                    com.Connection = con;
                    con.Open();
                    sqlT = con.BeginTransaction();
                    com.Transaction = sqlT;
                    com.Parameters.AddWithValue("studiesName", student.Studies);
                    var wynik = UseProcedure("checkIfExistsStudies", com, sqlT);
                    if (wynik.Count == 0) return new BadRequestResult();

                    com.CommandText = "SELECT 1 FROM Student WHERE Student.IndexNumber = @indexNumber";
                    com.Parameters.AddWithValue("indexNumber", student.IndexNumber);

                    var dr = com.ExecuteReader();
                    if (dr.Read()) return new BadRequestResult();
                    dr.Close();

                    com.CommandText = "DECLARE @datetmp date = PARSE(@bdate as date USING 'en-GB');" +
                                      " INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment)" +
                                      " VALUES (@indexNumber, @name, @lname, @datetmp, '1')";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("indexNumber", student.IndexNumber);
                    com.Parameters.AddWithValue("name", student.FirstName);
                    com.Parameters.AddWithValue("lname", student.LastName);
                    com.Parameters.AddWithValue("bdate", student.BirthDate);
                    com.ExecuteNonQuery();

                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("studiesName", student.Studies);
                    com.Parameters.AddWithValue("indexNumber", student.IndexNumber);
                    wynik = UseProcedure("enrollStudent", com, sqlT);

                    enrollment.IdEnrollment = wynik[0][0];
                    enrollment.IdStudy = wynik[0][2];
                    enrollment.Semester = wynik[0][1];
                    enrollment.StartDate = wynik[0][3];

                    sqlT.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    sqlT.Rollback();
                    return new BadRequestResult();
                }
            }

            ObjectResult objectResult = new ObjectResult(enrollment);
            objectResult.StatusCode = 201;
            return objectResult;
        }

        public IActionResult promoteStudents(StudiesInfo studies)
        {
            Enrollment enrollment = new Enrollment();
            using (SqlConnection con = new SqlConnection(sqlCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "PromoteStudents";
                com.Parameters.AddWithValue("Studies", studies.Studies);
                com.Parameters.AddWithValue("Semester", studies.Semester);

                var dr = com.ExecuteReader();

                dr.Read();
                if (dr.FieldCount==1)
                {
                    return new NotFoundResult();
                }
                enrollment.IdEnrollment = dr.GetString(0);
                enrollment.Semester = dr.GetString(1);
                enrollment.IdStudy = dr.GetString(2);
                enrollment.StartDate = dr.GetString(3);
                dr.Close();
            }

            ObjectResult objectResult = new ObjectResult(enrollment);
            objectResult.StatusCode = 201;
            return objectResult;

        }


        public List<string[]> UseProcedure(string nameOfProcedure, SqlCommand com, SqlTransaction sqlT)
        {
            List<string[]> wynik = new List<string[]>();

            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = nameOfProcedure;
            if (com.Connection.State != ConnectionState.Open) com.Connection.Open();

            var dr = com.ExecuteReader();

            while (dr.Read())
            {
                string[] tmp = new string[dr.FieldCount];
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    tmp[i] = dr.GetValue(i).ToString();
                }
                wynik.Add(tmp);
            }
            dr.Close();
            com.CommandType = CommandType.Text;
            com.Parameters.Clear();

            return wynik;
        }


    }
}
