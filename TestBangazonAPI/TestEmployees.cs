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
                var response = await client.GetAsync("/api/employees/");

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
                var employee = JsonConvert.DeserializeObject<Employee>(responseBody);
              
                // ASSERT
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employee.Id == 1);
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
                    DepartmentId = 1,
                    IsSupervisor = false
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

        [Fact]
        public async Task Test_Modify_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Random random = new Random();
                int randomNum = random.Next(0, 100);
                string newFirstName = $"Andy {randomNum.ToString()}";
                Employee modifiedEmployee = new Employee()
                {
                    FirstName = newFirstName,
                    LastName = "Collins",
                    DepartmentId = 1,
                    IsSupervisor = true
                };
                var employeeAsJSON = JsonConvert.SerializeObject(modifiedEmployee);

                var response = await client.PutAsync(
                    "/api/employees/1",
                    new StringContent(employeeAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getEmployee = await client.GetAsync("/api/employees/1");
                getEmployee.EnsureSuccessStatusCode();

                string getEmployeeBody = await getEmployee.Content.ReadAsStringAsync();
                var newEmployee = JsonConvert.DeserializeObject<Employee>(getEmployeeBody);


                Assert.Equal(HttpStatusCode.OK, getEmployee.StatusCode);
                Assert.Equal(newFirstName, newEmployee.FirstName);
            }
        }






    }
}
