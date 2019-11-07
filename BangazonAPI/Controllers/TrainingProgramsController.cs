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
                    else
                    {
                        cmd.CommandText = @"SELECT id, name, startDate, endDate, maxAttendees
                                                FROM TrainingProgram";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                        while (reader.Read())
                        {
                            TrainingProgram trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("maxAttendees"))
                            };

                            trainingPrograms.Add(trainingProgram);
                        }

                        reader.Close();

                        return Ok(trainingPrograms);
                    }

                }
            }
        }

    }
}
