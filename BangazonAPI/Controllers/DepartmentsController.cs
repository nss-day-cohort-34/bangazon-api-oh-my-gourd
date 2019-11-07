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
    public class DepartmentsController : ControllerBase
    {

        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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

        // GET api/departments
        [HttpGet]
        public async Task<IActionResult> Get(string _include, string _filter, int _gt, int _lt)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "employees")
                    {
                        cmd.CommandText = @"SELECT d.Id AS TheDepartmentId, d.Name, d.Budget, e.Id AS EmployeeId, e.FirstName, e.LastName, e.DepartmentId
                                        FROM Department d LEFT JOIN Employee e 
                                        ON d.Id = e.DepartmentId";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, Department> departments = new Dictionary<int, Department>();
                        while (reader.Read())
                        {
                            int departmentId = reader.GetInt32(reader.GetOrdinal("TheDepartmentId"));
                            if (!departments.ContainsKey(departmentId))
                            {
                                Department department = new Department
                                {
                                    Id = departmentId,
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetDecimal(reader.GetOrdinal("Budget"))
                                };
                                departments.Add(departmentId, department);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("DepartmentId")))
                            {
                                int employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                                Employee employee = new Employee()
                                {
                                    Id = employeeId,
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                                };
                                departments[departmentId].Employees.Add(employee);
                            }

                        }

                        reader.Close();

                        return Ok(departments.Values);
                    }
                    else if (_filter == "budget")
                    {
                        if (_gt > 0)
                        {
                            cmd.CommandText = @"SELECT Id, Name, Budget
                                            FROM Department
                                            WHERE Budget >= @minimum ";
                            cmd.Parameters.Add(new SqlParameter("@minimum", _gt));
                            SqlDataReader reader = await cmd.ExecuteReaderAsync();

                            List<Department> departments = new List<Department>();
                            while (reader.Read())
                            {
                                Department department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetDecimal(reader.GetOrdinal("Budget"))
                                };

                                departments.Add(department);
                            }

                            reader.Close();

                            return Ok(departments);
                        }
                        else if (_lt > 0)
                        {
                            cmd.CommandText = @"SELECT Id, Name, Budget
                                            FROM Department
                                            WHERE Budget <= @maximum ";
                            cmd.Parameters.Add(new SqlParameter("@maximum", _lt));
                            SqlDataReader reader = await cmd.ExecuteReaderAsync();

                            List<Department> departments = new List<Department>();
                            while (reader.Read())
                            {
                                Department department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetDecimal(reader.GetOrdinal("Budget"))
                                };

                                departments.Add(department);
                            }

                            reader.Close();

                            return Ok(departments);
                        }
                        return NotFound();
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT Id, Name, Budget
                                        FROM Department";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        List<Department> departments = new List<Department>();
                        while (reader.Read())
                        {
                            Department department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetDecimal(reader.GetOrdinal("Budget"))
                            };

                            departments.Add(department);
                        }

                        reader.Close();

                        return Ok(departments);
                    }

                }
            }
        }

        //GET api/departments/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Budget
                                        FROM Department WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Department department = null;
                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetDecimal(reader.GetOrdinal("Budget"))
                        };
                    }

                    reader.Close();

                    return Ok(department);
                }
            }
        }
        // POST api/departments
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Department (Name, Budget)
                        OUTPUT INSERTED.Id
                        VALUES (@name, @budget)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));
                    

                    department.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetDepartment", new { id = department.Id }, department);
                }
            }
        }
        // PUT api/departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Department
                            SET Name = @name, Budget = @budget
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));


                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(department);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Department WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}