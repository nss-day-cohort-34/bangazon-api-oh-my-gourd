using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class TestDepartments
    {

        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/departments");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Departments_And_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/departments?_include=employees");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_A_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/departments/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(department.Id == 2);
            }
        }

        [Fact]
        public async Task Create_A_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                Department newDepartment = new Department()
                {
                    Name = "Sales",
                    Budget = 1500000,
                    SupervisorId = 2
                };
                var departmentAsJSON = JsonConvert.SerializeObject(newDepartment);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/departments",
                    new StringContent(departmentAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(department.Name == "Sales");
            }
        }


        [Fact]
        public async Task Modify_A_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                int newSupervisorId = 3;
                Department modifiedDepartment = new Department()
                {
                    Name = "C#",
                    Budget = 1000000,
                    SupervisorId = newSupervisorId
                };
                var departmentAsJSON = JsonConvert.SerializeObject(modifiedDepartment);

                var response = await client.PutAsync(
                    "/api/departments/3",
                    new StringContent(departmentAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getDepartment = await client.GetAsync("/api/departments/3");
                getDepartment.EnsureSuccessStatusCode();

                string getDepartmentBody = await getDepartment.Content.ReadAsStringAsync();
                Department newDepartment = JsonConvert.DeserializeObject<Department>(getDepartmentBody);

                Assert.Equal(HttpStatusCode.OK, getDepartment.StatusCode);
                Assert.Equal(modifiedDepartment.SupervisorId, newDepartment.SupervisorId);
            }
        }

    }
}
