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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
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

        // GET api/customers
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
                    else if (_include == "payments")
                    {
                        return Ok();
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT Id, FirstName, LastName, CreationDate, LastActiveDate 
                                        FROM Customer";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        List<Customer> customers = new List<Customer>();
                        while (reader.Read())
                        {
                            Customer customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate"))
                            };

                            customers.Add(customer);
                        }

                        reader.Close();

                        return Ok(customers);
                    }

                }
            }
        }

        //GET api/customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, CreationDate, LastActiveDate 
                                        FROM Customer WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Customer customer = null;
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                            LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate"))
                        };
                    }

                    reader.Close();

                    return Ok(customer);
                }
            }
        }

        // POST api/customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate)
                        OUTPUT INSERTED.Id
                        VALUES (@firstName, @lastName, @creationDate, @lastActiveDate)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));
                    cmd.Parameters.Add(new SqlParameter("@creationDate", customer.CreationDate));
                    cmd.Parameters.Add(new SqlParameter("@lastActiveDate", customer.LastActiveDate));

                    customer.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
                }
            }
        }

        // PUT api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Customer
                            SET FirstName = @firstName, LastName = @lastName, CreationDate = @creationDate, LastActiveDate = @lastActiveDate
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));
                        cmd.Parameters.Add(new SqlParameter("@creationDate", customer.CreationDate));
                        cmd.Parameters.Add(new SqlParameter("@lastActiveDate", customer.LastActiveDate));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(customer);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
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
                    cmd.CommandText = @"SELECT o.customerId, p.SellerId 
                                        FROM [Order] o 
                                        FULL OUTER JOIN Product p 
                                        ON p.SellerId = @id 
                                        WHERE o.CustomerId = @id OR p.SellerId = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        reader.Close();
                        return new ContentResult() { Content = "Error 418: I'm a teapot.", StatusCode = 418 };
                    }
                    else
                    {
                        reader.Close();
                        cmd.CommandText = @"DELETE FROM Customer
                                        WHERE id = @id
                                       ";
                        cmd.ExecuteNonQuery();
                        return Ok($"Deleted item at index {id}");
                    }

                }
            }
        }

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
