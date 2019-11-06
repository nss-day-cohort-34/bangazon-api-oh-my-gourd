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
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;
        public OrdersController(IConfiguration config)
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

        // GET api/orders
        [HttpGet]
        public async Task<IActionResult> Get(string completed, string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (completed == "false")
                    {
                        cmd.CommandText = @"SELECT Id, CustomerId, PaymentTypeId, Total, IsCompleted
                                            FROM [Order]
                                            WHERE IsCompleted = 0";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        List<Order> orders = new List<Order>();
                        while (reader.Read())
                        {
                            Order order = new Order()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted"))
                            };

                            orders.Add(order);
                        }
                        reader.Close();

                        return Ok(orders);
                    }
                    else if (_include == "products")
                    {
                        cmd.CommandText = @"SELECT o.Id AS OrderId, o.CustomerId, o.PaymentTypeId, o.Total, o.IsCompleted, 
                                            op.ProductId, p.Title, p.Description, p.Price, p.ProductTypeId, p.Quantity, p.SellerId
                                            FROM [Order] o
                                            LEFT JOIN OrderProduct op on o.Id = op.OrderId     
                                            LEFT JOIN Product p on op.ProductId = p.Id";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, Order> orders = new Dictionary<int, Order>();
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                            if (!orders.ContainsKey(orderId))
                            {
                                Order order = new Order()
                                {
                                    Id = orderId,
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                    IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted"))
                                };

                                orders.Add(orderId, order);
                            }
                            
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                int productId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                                Product product = new Product()
                                {
                                    Id = productId,
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    SellerId = reader.GetInt32(reader.GetOrdinal("SellerId")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                };
                                orders[orderId].Products.Add(product);
                            }
                        }
                        reader.Close();

                        return Ok(orders.Values);
                    }
                    //else if (_include == "customers")
                    //{

                    //}
                    else
                    {
                        cmd.CommandText = @"SELECT Id, CustomerId, PaymentTypeId, Total, IsCompleted
                                        FROM [Order]";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        List<Order> orders = new List<Order>();
                        while (reader.Read())
                        {
                            Order order = new Order()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted"))
                            };

                            orders.Add(order);
                        }
                        reader.Close();

                        return Ok(orders);

                    }


                }
            }
        }

        // GET api/orders/1
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CustomerId, PaymentTypeId, Total, IsCompleted
                                        FROM [Order]
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Order anOrder = null;
                    if (reader.Read())
                    {
                        anOrder = new Order()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted"))
                        };
                    }

                    reader.Close();

                    return Ok(anOrder);
                }
            }
        }

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO [Order] (CustomerId, PaymentTypeId, Total, IsCompleted)
                        OUTPUT INSERTED.Id
                        VALUES (@customerId, @paymentTypeId, @total, @isCompleted) 
                    ";
                    cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));
                    cmd.Parameters.Add(new SqlParameter("@total", order.Total));
                    cmd.Parameters.Add(new SqlParameter("@isCompleted", order.IsCompleted));

                    order.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
                }
            }
        }

        // PUT api/orders/3
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE [Order] 
                            SET CustomerId = @customerId, PaymentTypeId = @paymentTypeId, Total = @total, IsCompleted = @isCompleted
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));
                        cmd.Parameters.Add(new SqlParameter("@total", order.Total));
                        cmd.Parameters.Add(new SqlParameter("@isCompleted", order.IsCompleted));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(order);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
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
                    cmd.CommandText = @"DELETE FROM Order 
                                        WHERE id = @id
                                        DELETE FROM OrderProduct
                                        WHERE OrderId = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));


                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return Ok();
                    }
                    throw new Exception("No rows affected");
                }
            }
        }

        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM [Order] WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}