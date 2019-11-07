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
    public class PaymentTypesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public PaymentTypesController(IConfiguration config)
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

        // GET api/paymentTypes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, AcctNumber, CustomerId
                                        FROM PaymentType";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<PaymentType> paymentTypes = new List<PaymentType>();
                    while (reader.Read())
                    {
                        PaymentType paymentType = new PaymentType()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            AccountNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                        };

                        paymentTypes.Add(paymentType);
                    }

                    reader.Close();

                    return Ok(paymentTypes);
                }
            }
        }

        // GET api/paymentTypes/1
        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, AcctNumber, CustomerId
                                        FROM PaymentType
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    PaymentType aPaymentType = null;

                    if (reader.Read())
                    {
                        aPaymentType = new PaymentType()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            AccountNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                    }

                    reader.Close();

                    return Ok(aPaymentType);
                }
            }
        }

        // POST api/paymentTypes
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentType paymentType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO PaymentType (Name, AcctNumber, CustomerId)
                        OUTPUT INSERTED.Id
                        VALUES (@Name, @acctNumber, @customerId)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@Name", paymentType.Name));
                    cmd.Parameters.Add(new SqlParameter("@acctNumber", paymentType.AccountNumber));
                    cmd.Parameters.Add(new SqlParameter("@customerId", paymentType.CustomerId));

                    paymentType.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetPaymentType", new { id = paymentType.Id }, paymentType);
                }
            }
        }

        // PUT api/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PaymentType paymentType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE PaymentType
                            SET Name = @Name, AcctNumber = @acctNumber, CustomerId = @customerId
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@Name", paymentType.Name));
                        cmd.Parameters.Add(new SqlParameter("@acctNumber", paymentType.AccountNumber));
                        cmd.Parameters.Add(new SqlParameter("@customerId", paymentType.CustomerId));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok(paymentType);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!PaymentTypeExists(id))
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
                    cmd.CommandText = @"SELECT pt.Id AS PaymentTypeId, o.Id AS OrderId
                                        FROM [Order] o
                                        LEFT JOIN PaymentType pt on o.PaymentTypeId = pt.Id
                                        WHERE pt.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
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
                    else
                    {
                        reader.Close();
                        cmd.CommandText = @"DELETE FROM PaymentType
                                        WHERE id = @id
                                       ";
                        cmd.ExecuteNonQuery();
                        return Ok($"Deleted item with id {id}");
                    }

                }
            }
        }

        private bool PaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM PaymentType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}