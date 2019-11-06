using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestEmployees
    {
        [Fact]
        public async Task Test_Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/employees");


                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employees.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {    
                // ARRANGE
            
                // ACT
                var response = await client.GetAsync("/api/employees/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
              
                // ASSERT
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employees[0].Id == 1);
                Assert.True(employees.Count == 1);
            }
        }

        [Fact]
        public async Task Test_Create_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                // ARRANGE
                Employee newEmployee = new Employee()
                {
                    FirstName = "Bill",
                    LastName = "Gates",
                    DepartmentId = 1
                };
                var employeeAsJSON = JsonConvert.SerializeObject(newEmployee);

                // ACT
                var response = await client.PostAsync(
                    "/api/employees",
                    new StringContent(employeeAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(responseBody);

                // ASSERT
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(employee.FirstName == "Bill");
            }
        }


    }
}
