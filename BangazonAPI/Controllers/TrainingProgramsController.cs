using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingProgramsController : ControllerBase
    {

        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/trainingPrograms
        [HttpGet]
        public async Task<IActionResult> Get(string completed)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (completed == "false")
                    {
                        DateTime currentDateTime = DateTime.Now;
                        cmd.Parameters.Add(new SqlParameter("@dateTime", currentDateTime));
                        cmd.CommandText = @"SELECT tp.id as TPID, tp.name as TPName, tp.startDate, tp.endDate, tp.maxAttendees, e.id as EID, e.firstName, e.lastName, e.isSupervisor, e.departmentId, d.Name, c.Make, c.Manufacturer, c.IsWorking, c.PurchaseDate, c.Id as CID
                                        FROM TrainingProgram tp LEFT JOIN EmployeeTraining et
                                        ON et.TrainingProgramId = tp.id
                                        Left join Employee e 
                                        on et.EmployeeId = e.id 
										left join Department d
										on e.DepartmentId = d.Id
										left join Computer c
										on c.EmployeeId = e.id
										where tp.EndDate >= @dateTime";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, TrainingProgram> trainingPrograms = new Dictionary<int, TrainingProgram>();
                        while (reader.Read())
                        {
                            int trainingProgramId = reader.GetInt32(reader.GetOrdinal("TPID"));
                            if (!trainingPrograms.ContainsKey(trainingProgramId))
                            {
                                TrainingProgram trainingProgram = new TrainingProgram
                                {
                                    Id = trainingProgramId,
                                    Name = reader.GetString(reader.GetOrdinal("TPName")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                                    MaxAttendees = reader.GetInt32(reader.GetOrdinal("maxAttendees"))
                                };
                                trainingPrograms.Add(trainingProgramId, trainingProgram);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("EID")))
                            {
                                int employeeId = reader.GetInt32(reader.GetOrdinal("EID"));
                                Employee employee = new Employee()
                                {
                                    Id = employeeId,
                                    FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentId")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("name"))
                                };
                                trainingPrograms[trainingProgramId].employees.Add(employee);
                                if (!reader.IsDBNull(reader.GetOrdinal("CID")))
                                {
                                    employee.Computer = new Computer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CID")),
                                        PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                        DecommissionDate = null,
                                        Make = reader.GetString(reader.GetOrdinal("Make")),
                                        Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                        IsWorking = reader.GetBoolean(reader.GetOrdinal("IsWorking")),
                                        EmployeeId = employeeId
                                    };

                                }
                            }


                        }

                        reader.Close();

                        return Ok(trainingPrograms.Values);
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT tp.id as TPID, tp.name as TPName, tp.startDate, tp.endDate, tp.maxAttendees, e.id as EID, e.firstName, e.lastName, e.isSupervisor, e.departmentId, d.Name, c.Make, c.Manufacturer, c.IsWorking, c.PurchaseDate, c.Id as CID
                                        FROM TrainingProgram tp LEFT JOIN EmployeeTraining et
                                        ON et.TrainingProgramId = tp.id
                                        Left join Employee e 
                                        on et.EmployeeId = e.id 
										left join Department d
										on e.DepartmentId = d.Id
										left join Computer c
										on c.EmployeeId = e.id";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, TrainingProgram> trainingPrograms = new Dictionary<int, TrainingProgram>();
                        while (reader.Read())
                        {
                            int trainingProgramId = reader.GetInt32(reader.GetOrdinal("TPID"));
                            if (!trainingPrograms.ContainsKey(trainingProgramId))
                            {
                                TrainingProgram trainingProgram = new TrainingProgram
                                {
                                    Id = trainingProgramId,
                                    Name = reader.GetString(reader.GetOrdinal("TPName")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                                    MaxAttendees = reader.GetInt32(reader.GetOrdinal("maxAttendees"))
                                };
                                trainingPrograms.Add(trainingProgramId, trainingProgram);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("EID")))
                            {
                                int employeeId = reader.GetInt32(reader.GetOrdinal("EID"));
                                Employee employee = new Employee()
                                {
                                    Id = employeeId,
                                    FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentId")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),
                                    DepartmentName = reader.GetString(reader.GetOrdinal("name"))
                                };
                                trainingPrograms[trainingProgramId].employees.Add(employee);
                                if (!reader.IsDBNull(reader.GetOrdinal("CID")))
                                {
                                    employee.Computer = new Computer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CID")),
                                        PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                        DecommissionDate = null,
                                        Make = reader.GetString(reader.GetOrdinal("Make")),
                                        Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                        IsWorking = reader.GetBoolean(reader.GetOrdinal("IsWorking")),
                                        EmployeeId = employeeId
                                    };

                                }
                            }


                        }

                        reader.Close();

                        return Ok(trainingPrograms.Values);
                    }

                }
            }
        }

        //GET api/trainingprograms/5
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.id as TPID, tp.name as TPName, tp.startDate, tp.endDate, tp.maxAttendees, e.id as EID, e.firstName, e.lastName, e.isSupervisor, e.departmentId, d.Name, c.Make, c.Manufacturer, c.IsWorking, c.PurchaseDate, c.Id as CID
                                        FROM TrainingProgram tp LEFT JOIN EmployeeTraining et
                                        ON et.TrainingProgramId = tp.id
                                        Left join Employee e
                                        on et.EmployeeId = e.id

                                        left join Department d

                                        on e.DepartmentId = d.Id

                                        left join Computer c

                                        on c.EmployeeId = e.id
                                        where tp.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, TrainingProgram> trainingPrograms = new Dictionary<int, TrainingProgram>();
                    while (reader.Read())
                    {
                        int trainingProgramId = reader.GetInt32(reader.GetOrdinal("TPID"));
                        if (!trainingPrograms.ContainsKey(trainingProgramId))
                        {
                            TrainingProgram trainingProgram = new TrainingProgram
                            {
                                Id = trainingProgramId,
                                Name = reader.GetString(reader.GetOrdinal("TPName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("maxAttendees"))
                            };
                            trainingPrograms.Add(trainingProgramId, trainingProgram);
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("EID")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("EID"));
                            Employee employee = new Employee()
                            {
                                Id = employeeId,
                                FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                                LastName = reader.GetString(reader.GetOrdinal("lastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("name"))
                            };
                            trainingPrograms[trainingProgramId].employees.Add(employee);
                            if (!reader.IsDBNull(reader.GetOrdinal("CID")))
                            {
                                employee.Computer = new Computer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CID")),
                                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                    DecommissionDate = null,
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                    IsWorking = reader.GetBoolean(reader.GetOrdinal("IsWorking")),
                                    EmployeeId = employeeId
                                };

                            }
                        }


                    }
                    reader.Close();

                    return Ok(trainingPrograms.Values);
                }
            }

        }

        // POST api/trainingPrograms
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                        OUTPUT INSERTED.Id
                        VALUES (@Name, @startDate, @EndDate, @maxAttendees)";
                    cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));

                    trainingProgram.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetTrainingProgram", new { id = trainingProgram.Id }, trainingProgram);
                }
            }
        }

        // PUT api/trainingPrograms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE TrainingProgram
                            SET Name = @Name, StartDate = @startDate, EndDate = @EndDate, MaxAttendees = @maxAttendees
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(trainingProgram);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select Count(et.EmployeeId) as EmployeesEnrolled, tp.id, tp.Name from TrainingProgram tp 
                                        left join EmployeeTraining et on tp.Id = et.TrainingProgramId 
                                        Group By tp.Id, tp.StartDate, tp.Name
                                        having tp.Id = @id and tp.StartDate > @dateTime and Count(et.EmployeeId) = 0";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    DateTime currentDateTime = DateTime.Now;
                    cmd.Parameters.Add(new SqlParameter("@dateTime", currentDateTime));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        reader.Close();
                        cmd.CommandText = @"DELETE FROM TrainingProgram
                                        WHERE id = @id
                                       ";
                        cmd.ExecuteNonQuery();
                        return Ok($"Deleted item at index {id}");
                    }
                    else
                    {
                        reader.Close();
                        return new ContentResult() { Content = @"Error 418: I'm a teapot.             
                                                                        ;,'
                                                                 _o_    ;:; '
                                                             ,-.'---`.__ ;
                                                            ((j`===== ',-'
                                                             `-\     /
                                                                `-= -'     ", StatusCode = 418 };
                    }

                }
            }
        }

        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }


    }
}
