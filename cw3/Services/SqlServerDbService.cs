using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private string sqlCon = "Data Source=db-mssql;Initial Catalog=s18728;Integrated Security=True";

        public IActionResult enrollStudent(Student student)
        {
            using (var con = new SqlConnection(sqlCon))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.Parameters.AddWithValue("studiesName", student.Studies);
                var wynik = UseProcedure("checkIfExistsStudies",com);
                if (wynik.Count == 0) return new BadRequestResult();

                com.CommandText = "SELECT 1 FROM Student WHERE Student.IndexNumber = @indexNumber";
                com.Parameters.AddWithValue("indexNumber", student.IndexNumber);

                var dr = com.ExecuteReader();
                if (dr.Read()) return  new BadRequestResult();
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
                UseProcedure("enrollStudent", com);

            }

            return new OkResult();
        }

        public List<string[]> UseProcedure(string nameOfProcedure, SqlCommand com)
        {
            List<string[]> wynik = new List<string[]>();
            
            {
                
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
            }

            return wynik;
        }

        
    }
}
