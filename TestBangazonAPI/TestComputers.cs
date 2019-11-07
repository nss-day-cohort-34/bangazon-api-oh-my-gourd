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
    public class TestComputers
    {

        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/computers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_A_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                var getAllResponse = await client.GetAsync("/api/computers");


                string getAllResponseBody = await getAllResponse.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(getAllResponseBody);


                /*
                    ACT
                */
                var response = await client.GetAsync($"/api/departments/{computers[0].Id}");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computer.Id == computers[0].Id);
            }
        }

        [Fact]
        public async Task Create_A_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                Computer newComputer = new Computer()
                {
                    PurchaseDate = new DateTime(2019,11,7),
                    DecommissionDate = null,
                    Make = "Area 51M",
                    Manufacturer = "Alienware",
                    EmployeeId = 1
                };
                var computerAsJSON = JsonConvert.SerializeObject(newComputer);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/computers",
                    new StringContent(computerAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);

                var getAllResponse = await client.GetAsync("/api/computers");


                string getAllResponseBody = await getAllResponse.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(getAllResponseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                //Check if the last item in the computers list has the same Id as the one we created
                Assert.True(computer.Id == computers[computers.Count - 1].Id);
            }
        }


        [Fact]
        public async Task Modify_A_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {

                var getAllResponse = await client.GetAsync("/api/computers");


                string getAllResponseBody = await getAllResponse.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(getAllResponseBody);
                /*
                    PUT section
                */
                string newMake = "Blade Pro";
                Computer modifiedComputer = new Computer()
                {
                    PurchaseDate = new DateTime(2019,11,5),
                    Make = newMake,
                    Manufacturer = "Razer",
                    EmployeeId = 1
                };
                var computerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                var response = await client.PutAsync(
                    $"/api/computers/{computers[0].Id}",
                    new StringContent(computerAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getComputer= await client.GetAsync($"/api/computers/{computers[0].Id}");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal(modifiedComputer.Make, newComputer.Make);
            }
        }

        [Fact]
        public async Task Test_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Computer newComputer = new Computer()
                {
                    PurchaseDate = new DateTime(2019, 11, 7),
                    DecommissionDate = null,
                    Make = "Area 51M",
                    Manufacturer = "Alienware",
                    EmployeeId = 1
                };
                var computerAsJSON = JsonConvert.SerializeObject(newComputer);

                var postResponse = await client.PostAsync(
                    "/api/computers",
                    new StringContent(computerAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await postResponse.Content.ReadAsStringAsync();

                var computer = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ACT
                */



                var deleteResponse = await client.DeleteAsync($"/api/computers/{computer.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }
    }
}
