using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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

        // GET api/employees
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id as 'TheEmployeeId', e.FirstName, e.LastName, e.DepartmentId,
                                        d.Name as 'DepartmentName', 
                                        c.Id as 'ComputerId', c.PurchaseDate, c.Make, c.Manufacturer, c.IsWorking
                                        FROM Employee e LEFT JOIN Department d ON d.Id = e.DepartmentId
                                                        LEFT JOIN Computer c ON e.Id = c.EmployeeId";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TheEmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                        };

                        // Check to see if the employee has a computer assigned to them
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            employee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                IsWorking = reader.GetBoolean(reader.GetOrdinal("IsWorking"))
                            };
                        }

                        employees.Add(employee);
                    }
                    reader.Close();

                    return Ok(employees);
                }
            }
        }

        // Get one employee from the DB by their id in the route
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id as 'TheEmployeeId', e.FirstName, e.LastName, e.DepartmentId,
                                        d.Name as 'DepartmentName', 
                                        c.Id as 'ComputerId', c.PurchaseDate, c.Make, c.Manufacturer, c.IsWorking
                                        FROM Employee e LEFT JOIN Department d ON d.Id = e.DepartmentId
                                                        LEFT JOIN Computer c ON e.Id = c.EmployeeId
                                        WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Employee employee = null;

                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TheEmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                        };

                        // Check to see if the employee has a computer assigned to them
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            employee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                IsWorking = reader.GetBoolean(reader.GetOrdinal("IsWorking"))
                            };
                        }

                    }
                    reader.Close();

                    return Ok(employee);
                }
            }
        }

        // POST: api/employees
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee newEmployee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, DepartmentId)
                                           OUTPUT INSERTED.Id
                                        VALUES (@firstName, @lastName, @departmentId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newEmployee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newEmployee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", newEmployee.DepartmentId));

                    newEmployee.Id = (int)await cmd.ExecuteScalarAsync();
                    return CreatedAtRoute("GetEmployee", new { id = newEmployee.Id }, newEmployee);
                }
            }
        }

        // PUT api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Employee
                            SET FirstName = @firstName, LastName = @lastName, DepartmentId = @departmentId
                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(employee);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool EmployeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }


    }
}