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
    public class ComputersController : ControllerBase
    {

        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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

        // GET api/computers
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId 
                                        FROM Computer";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        int decommissionDateIndex = reader.GetOrdinal("DecomissionDate");
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            // If the Decommission date is null in the DB, then return the date 01/01/01 because ASP.NET doesn't know how to convert null values into JSON
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"))
                        };
                        if (!reader.IsDBNull(decommissionDateIndex))
                        {
                            computer.DecommissionDate = reader.GetDateTime(decommissionDateIndex);
                        } 

                        computers.Add(computer);
                    }

                    reader.Close();

                    return Ok(computers);


                }
            }
        }

        //GET api/computers/5
        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId 
                                        FROM Computer WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Computer computer = null;
                    if (reader.Read())
                    {
                        int decommissionDateIndex = reader.GetOrdinal("DecomissionDate");
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            // If the Decommission date is null in the DB, then return the date 01/01/01 because ASP.NET doesn't know how to convert null values into JSON
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"))
                        };
                        if (!reader.IsDBNull(decommissionDateIndex))
                        {
                            computer.DecommissionDate = reader.GetDateTime(decommissionDateIndex);
                        }
                    }

                    reader.Close();

                    return Ok(computer);
                }
            }
        }
        // POST api/computers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Computer computer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId, IsWorking)
                        OUTPUT INSERTED.Id
                        VALUES (@purchaseDate, @decommissionDate, @make, @manufacturer, @employeeId, @IsWorking)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                    if (computer.DecommissionDate == null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@decommissionDate", DBNull.Value));
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@decommissionDate", computer.DecommissionDate));
                    }
                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                    cmd.Parameters.Add(new SqlParameter("@employeeId", computer.EmployeeId));
                    cmd.Parameters.Add(new SqlParameter("@isWorking", computer.IsWorking));

                    computer.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetCustomer", new { id = computer.Id }, computer);
                }
            }
        }
    }
}