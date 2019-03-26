using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentExercisesPt5.Models;

namespace StudentExercisesPt5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        // Connection established
        public SqlConnection Connection 
        {
            get
            {
                string connectionSTring = "Server=localhost\\SQLExpress;Database=StudentExercisesDB;Integrated Security=true";
                return new SqlConnection(connectionSTring);
            }
        }
        /***********************
         GET
        ***********************/
        // GET api/Instructors
        [HttpGet]
        public IEnumerable<Instructor> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.id, i.firstname, i.lastname,
                                               i.slackhandle, i.cohortId, c.name as cohortname
                                          FROM Instructor i INNER JOIN Cohort c ON i.cohortid = c.id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();
                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                            LastName = reader.GetString(reader.GetOrdinal("lastname")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("slackhandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("cohortid")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cohortid")),
                                Name = reader.GetString(reader.GetOrdinal("cohortname"))
                            }
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();
                    return instructors;
                }
            }
        }
        /***********************
        GET BY INSTRUCTOR ID
        ***********************/
        // GET api/Instructors/{id}
        [HttpGet("{id}", Name = "GetInstructor")]
        public Instructor Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.id, i.firstname, i.lastname,
                                               i.slackhandle, i.cohortId, c.name as cohortname
                                          FROM Instructor i INNER JOIN Cohort c ON i.cohortid = c.id
                                         WHERE i.id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;
                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                            LastName = reader.GetString(reader.GetOrdinal("lastname")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("slackhandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("cohortid")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cohortid")),
                                Name = reader.GetString(reader.GetOrdinal("cohortname"))
                            }
                        };
                    }

                    reader.Close();
                    return instructor;
                }
            }
        }
        /***********************
        POST
        ***********************/
        // POST api/Instructors/
        [HttpPost]
        public ActionResult Post([FromBody] Instructor newInstructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO instructor (firstname, lastname, slackhandle, cohortid)
                                             OUTPUT INSERTED.Id
                                             VALUES (@firstname, @lastname, @slackhandle, @cohortid)";
                    cmd.Parameters.Add(new SqlParameter("@firstname", newInstructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", newInstructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackhandle", newInstructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortid", newInstructor.CohortId));

                    int newId = (int)cmd.ExecuteScalar();
                    newInstructor.Id = newId;
                    return CreatedAtRoute("GetInstructor", new { id = newId }, newInstructor);
                }
            }
        }
        /***********************
        PUT
        ***********************/
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Instructor instructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE instructor 
                                           SET firstname = @firstname, 
                                               lastname = @lastname,
                                               slackhandle = @slackhandle, 
                                               cohortid = @cohortid
                                         WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@firstname", instructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", instructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackhandle", instructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortid", instructor.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
        /***********************
        DELETE
        ***********************/
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM instructor WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
