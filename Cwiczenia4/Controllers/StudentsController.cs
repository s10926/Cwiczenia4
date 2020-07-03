using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cwiczenia4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia4.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private const String ConString = "Data Source=db-mssql;Initial Catalog=s10926;Integrated Security=True";

        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                com.CommandText = @"SELECT s.FirstName,
				                		   s.LastName,
						                   s.BirthDate,
						                   ss.Name,
						                   e.Semester
					                FROM   Student s
					                JOIN   Enrollment e
					                ON     s.IdEnrollment = e.IdEnrollment
					                JOIN   Studies ss
					                ON     e.IdStudy = ss.IdStudy";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
                    st.Name = dr["Name"].ToString();
                    st.Semester = int.Parse(dr["Semester"].ToString());
                    list.Add(st);
                }
            }

            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetEnrollments(string id)
        {
            var list = new List<Enrollment>();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                com.CommandText = @"SELECT e.Semester,
						                   ss.Name,
						                   e.StartDate
					                FROM   Student s
					                JOIN   Enrollment e
					                ON     s.IdEnrollment = e.IdEnrollment
					                JOIN   Studies ss
					                ON     e.IdStudy = ss.IdStudy
					                WHERE  s.IndexNumber = @id";

                SqlParameter par = new SqlParameter();
                par.Value = id;
                par.ParameterName = "id";
                com.Parameters.Add(par);

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var en = new Enrollment();
                    en.Semester = int.Parse(dr["Semester"].ToString());
                    en.Name = dr["Name"].ToString();
                    en.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                    list.Add(en);
                }
            }

            return Ok(list);
        }
    }
}