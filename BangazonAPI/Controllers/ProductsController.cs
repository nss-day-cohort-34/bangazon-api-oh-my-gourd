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
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
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
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ProductTypeId, Quantity, Price, Title, Description, SellerId
                                        FROM Product";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Product> products = new List<Product>();
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            Price = (double)reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            SellerId = reader.GetInt32(reader.GetOrdinal("SellerId"))
                        };

                        products.Add(product);
                    }

                    reader.Close();

                    return Ok(products);

                }
            }
        }

        //GET api/customers/5
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ProductTypeId, Quantity, Price, Title, Description, SellerId
                                        FROM Product WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Product product = null;
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            Price = (double)reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            SellerId = reader.GetInt32(reader.GetOrdinal("SellerId"))
                        };
                    }

                    reader.Close();

                    return Ok(product);
                }
            }
        }

        // POST api/customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Product (ProductTypeId, Quantity, Price, Title, Description, SellerId)
                        OUTPUT INSERTED.Id
                        VALUES (@productTypeId, @quantity, @price, @title, @description, @sellerId)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));
                    cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@sellerId", product.SellerId));

                    product.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
                }
            }
        }

        // PUT api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product product)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Product
                            SET ProductTypeId = @productTypeId, Quantity = @quantity, Price = @price, Title = @title, Description = @description, SellerId = @sellerId
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                        cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));
                        cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                        cmd.Parameters.Add(new SqlParameter("@sellerId", product.SellerId));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(product);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Product WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }

    }
}
