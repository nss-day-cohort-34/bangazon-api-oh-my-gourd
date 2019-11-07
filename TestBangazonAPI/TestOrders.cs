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
    public class TestOrders
    {
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Open_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders?completed=false");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Orders_With_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders?_include=products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_All_Orders_With_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders?_include=customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(order.Id == 1);
            }
        }

        [Fact]
        public async Task Create_An_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                Order newOrder = new Order()
                {
                    CustomerId = 1,
                    IsCompleted = false
                };
                var orderAsJSON = JsonConvert.SerializeObject(newOrder);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/orders/post",
                    new StringContent(orderAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(order.CustomerId == 1);
            }
        }

        [Fact]
        public async Task Modify_An_Order()
        {
            using (var client = new APIClientProvider().Client)
            {

                var getAllResponse = await client.GetAsync("/api/orders");


                string getAllResponseBody = await getAllResponse.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(getAllResponseBody);
                /*
                    PUT section
                */
                int newPaymentTypeId= 1;
                Order modifiedOrder = new Order()
                {
                    CustomerId = 3,
                    PaymentTypeId = newPaymentTypeId,
                    IsCompleted = false,
                    Total = 200
                };
                var orderAsJSON = JsonConvert.SerializeObject(modifiedOrder);

                var response = await client.PutAsync(
                    $"/api/orders/{orders[0].Id}",
                    new StringContent(orderAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getOrder= await client.GetAsync($"/api/orders/{orders[0].Id}");
                getOrder.EnsureSuccessStatusCode();

                string getOrderBody = await getOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getOrderBody);

                Assert.Equal(HttpStatusCode.OK, getOrder.StatusCode);
                Assert.Equal(newPaymentTypeId, newOrder.PaymentTypeId);
            }
        }

        [Fact]
        public async Task Test_Delete_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Order newOrder = new Order()
                {
                    CustomerId = 1,
                    IsCompleted = false
                };
                var orderAsJSON = JsonConvert.SerializeObject(newOrder);

                var postResponse = await client.PostAsync(
                    "/api/orders/post",
                    new StringContent(orderAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await postResponse.Content.ReadAsStringAsync();

                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                /*
                    ACT
                */



                var deleteResponse = await client.DeleteAsync($"/api/orders/{order.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }
    }
}
