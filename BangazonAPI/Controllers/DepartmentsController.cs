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
        public async Task<IActionResult> Get(string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "products")
                    {
                        cmd.CommandText = @"SELECT c.Id, c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate, p.Id AS ProductId, p.SellerId, p.Title, p.Description, p.Price, p.Quantity, p.ProductTypeId 
                                        FROM Customer c LEFT JOIN Product p 
                                        ON p.SellerId = c.Id";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
                        while (reader.Read())
                        {
                            int customerId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!customers.ContainsKey(customerId))
                            {
                                Customer customer = new Customer
                                {
                                    Id = customerId,
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                    LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate"))
                                };
                                customers.Add(customerId, customer);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                int productId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                                Product product = new Product()
                                {
                                    Id = productId,
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    SellerId = reader.GetInt32(reader.GetOrdinal("SellerId")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    Price = (double)reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                };
                                customers[customerId].Products.Add(product);
                            }

                        }

                        reader.Close();

                        return Ok(customers.Values);
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT Id, Name, Budget, SupervisorId
                                        FROM Department";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        List<Department> departments = new List<Department>();
                        while (reader.Read())
                        {
                            Department department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                SupervisorId = reader.GetInt32(reader.GetOrdinal("SupervisorId"))
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
                    cmd.CommandText = @"SELECT Id, Name, Budget, SupervisorId
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
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            SupervisorId = reader.GetInt32(reader.GetOrdinal("SupervisorId"))
                        };
                    }

                    reader.Close();

                    return Ok(department);
                }
            }
        }
    }
}