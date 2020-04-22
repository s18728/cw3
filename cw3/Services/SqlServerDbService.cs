using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using cw3.Models;
using cw3.Security;
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
                    var wynik = UseProcedure("checkIfExistsStudies", com);
                    if (wynik.Count == 0) return new BadRequestResult();

                    com.CommandText = "SELECT 1 FROM Student WHERE Student.IndexNumber = @indexNumber";
                    com.Parameters.AddWithValue("indexNumber", student.IndexNumber);

                    var dr = com.ExecuteReader();
                    if (dr.Read()) return new BadRequestResult();
                    dr.Close();

                    com.CommandText = "DECLARE @datetmp date = PARSE(@bdate as date USING 'en-GB');" +
                                      " INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment, Password, Salt)" +
                                      " VALUES (@indexNumber, @name, @lname, @datetmp, '1', @pass, @salt)";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("indexNumber", student.IndexNumber);

                    var salt = Salter.CreateSalt();
                    var pass = Salter.CreateHash("pas" + student.IndexNumber, salt);

                    com.Parameters.AddWithValue("pass",pass);
                    com.Parameters.AddWithValue("salt", salt);
                    com.Parameters.AddWithValue("name", student.FirstName);
                    com.Parameters.AddWithValue("lname", student.LastName);
                    com.Parameters.AddWithValue("bdate", student.BirthDate);
                    com.ExecuteNonQuery();

                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("studiesName", student.Studies);
                    com.Parameters.AddWithValue("indexNumber", student.IndexNumber);
                    wynik = UseProcedure("enrollStudent", com);

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

                
                com.Parameters.AddWithValue("Studies", studies.Studies);
                com.Parameters.AddWithValue("Semester", studies.Semester);
                var wynik = UseProcedure("PromoteStudents", com);

                if (wynik[0][0].Equals("404"))
                {
                    return new NotFoundResult();
                }
                enrollment.IdEnrollment = wynik[0][0];
                enrollment.IdStudy = wynik[0][2];
                enrollment.Semester = wynik[0][1];
                enrollment.StartDate = wynik[0][3];

            }

            ObjectResult objectResult = new ObjectResult(enrollment);
            objectResult.StatusCode = 201;
            return objectResult;

        }

        public bool checkIfStudentExists(string indeks)
        {
            using (SqlConnection con = new SqlConnection(sqlCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT 1 FROM Student WHERE Student.IndexNumber = @indeks";
                com.Parameters.AddWithValue("indeks", indeks);

                var dr =com.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    dr.Close();
                    return true;
                }
                else
                {
                    dr.Close();
                    return false;
                }
            }
        }

        public string getStudentsString()
        {
            StringBuilder students = new StringBuilder();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "SELECT IndexNumber, FirstName, LastName, BirthDate, Semester, Name FROM Student " +
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
                    st.Studies = dr["Name"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    students.AppendLine(st.ToString());
                }

            }

            return students.ToString();
        }

        public Student getStudent(string indeks)
        {
            var st = new Student();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "SELECT IndexNumber, Password, FirstName, LastName, BirthDate, Semester, Name FROM Student " +
                    "INNER JOIN Enrollment ON Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "INNER JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy WHERE Student.IndexNumber = @indeks";

                com.Parameters.AddWithValue("indeks", indeks);

                con.Open();
                var dr = com.ExecuteReader();
                dr.Read();


                st.FirstName = dr["FirstName"].ToString();
                st.LastName = dr["LastName"].ToString();
                st.BirthDate = dr["BirthDate"].ToString();
                st.Semester = dr["Semester"].ToString();
                st.Studies = dr["Name"].ToString();
                st.IndexNumber = dr["IndexNumber"].ToString();
                st.Password = dr["Password"].ToString();

            }

            return st;
        }

        public string getEnrollments(string id)
        {
            StringBuilder enrollments = new StringBuilder();

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "SELECT * FROM Enrollment INNER JOIN Student ON Enrollment.IdEnrollment = Student.IdEnrollment WHERE Student.IndexNumber = @id";
                com.Parameters.AddWithValue("id", id);

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var en = new Enrollment();
                    en.IdEnrollment = dr["IdEnrollment"].ToString();
                    en.Semester = dr["Semester"].ToString();
                    en.IdStudy = dr["IdStudy"].ToString();
                    en.StartDate = dr["StartDate"].ToString();
                    enrollments.AppendLine(en.ToString());
                }
            }
            return enrollments.ToString();
        }

        public string setRefreshToken(string indeks, string token)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "UPDATE Student SET RefToken = @token WHERE Student.IndexNumber = @indeks";
                com.Parameters.AddWithValue("indeks", indeks);
                com.Parameters.AddWithValue("token", token);

                con.Open();
                com.ExecuteNonQuery();
            }

            return token;
        }

        public Student getStudentFromRefreshToken(string token)
        {
            using (SqlConnection con = new SqlConnection(sqlCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT IndexNumber, RefToken FROM Student WHERE Student.RefToken = @token";
                com.Parameters.AddWithValue("token", token);

                var dr = com.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    return getStudent(dr["IndexNumber"].ToString());
                }
                else
                {
                    return null;
                }
            }
        }

        public void setSalt(string indeks, string salt)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "UPDATE Student SET Salt = @salt WHERE Student.IndexNumber = @indeks";
                com.Parameters.AddWithValue("indeks", indeks);
                com.Parameters.AddWithValue("salt", salt);

                con.Open();
                com.ExecuteNonQuery();
            }
        }

        public string getSalt(string indeks)
        {
            using (SqlConnection con = new SqlConnection(sqlCon))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT IndexNumber, Salt FROM Student WHERE Student.IndexNumber = @indeks";
                com.Parameters.AddWithValue("indeks", indeks);

                var dr = com.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    return dr["Salt"].ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public void setPassword(string indeks, string pass)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "UPDATE Student SET Password = @pass WHERE Student.IndexNumber = @indeks";
                com.Parameters.AddWithValue("indeks", indeks);
                com.Parameters.AddWithValue("pass", pass);

                con.Open();
                com.ExecuteNonQuery();
            }
        }

        public void hashAllPasswords()
        {
            Console.Write("hello");
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT IndexNumber, Password FROM Student ";

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var salt = Salter.CreateSalt();
                    setSalt(dr["IndexNumber"].ToString(), salt);
                    var pass = Salter.CreateHash(dr["Password"].ToString(), salt);
                    setPassword(dr["IndexNumber"].ToString(), pass);
                }
                dr.Close();
            }
        }


        public List<string[]> UseProcedure(string nameOfProcedure, SqlCommand com)
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
