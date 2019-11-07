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
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Customers_And_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers?_include=products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Customers_And_Payments()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers?_include=payments");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_A_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customer.Id == 2);
            }
        }

        [Fact]
        public async Task Create_A_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                Customer newCustomer = new Customer()
                {
                    FirstName = "Will",
                    LastName = "Wilkinson",
                    CreationDate = new DateTime(2019,11,02),
                    LastActiveDate = new DateTime(2019,11,06)
                };
                var customerAsJSON = JsonConvert.SerializeObject(newCustomer);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/customers",
                    new StringContent(customerAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(customer.FirstName == "Will");
            }
        }


        [Fact]
        public async Task Modify_A_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                string newFirstName = "Bill";
                Customer modifiedCustomer = new Customer()
                {
                    FirstName = newFirstName,
                    LastName = "Wilkinson",
                    CreationDate = new DateTime(2019, 11, 02),
                    LastActiveDate = new DateTime(2019, 11, 06)
                };
                var customerAsJSON = JsonConvert.SerializeObject(modifiedCustomer);

                var response = await client.PutAsync(
                    "/api/customers/5",
                    new StringContent(customerAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getCustomer= await client.GetAsync("/api/customers/5");
                getCustomer.EnsureSuccessStatusCode();

                string getCustomerBody = await getCustomer.Content.ReadAsStringAsync();
                Customer newCustomer = JsonConvert.DeserializeObject<Customer>(getCustomerBody);

                Assert.Equal(HttpStatusCode.OK, getCustomer.StatusCode);
                Assert.Equal(newFirstName, newCustomer.FirstName);
            }
        }

        [Fact]
        public async Task Test_Delete_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Customer newCustomer = new Customer()
                {
                    FirstName = "Will",
                    LastName = "Wilkinson",
                    CreationDate = new DateTime(2019, 11, 02),
                    LastActiveDate = new DateTime(2019, 11, 06)
                };
                var customerAsJSON = JsonConvert.SerializeObject(newCustomer);

                var postResponse = await client.PostAsync(
                    "/api/customers",
                    new StringContent(customerAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await postResponse.Content.ReadAsStringAsync();

                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                /*
                    ACT
                */



                var deleteResponse = await client.DeleteAsync($"/api/customers/{customer.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }
    }
}
