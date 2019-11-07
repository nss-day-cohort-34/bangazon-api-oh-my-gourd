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
    public class TestPaymentTypes
    {
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymentTypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_A_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymentTypes/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentType.Id == 1);
            }
        }

        [Fact]
        public async Task Create_A_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                PaymentType newPaymentType = new PaymentType()
                {
                    Name = "chase",
                    AccountNumber = "38275-29275",
                    CustomerId = 3
                };
                var paymentTypeAsJSON = JsonConvert.SerializeObject(newPaymentType);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/paymentTypes",
                    new StringContent(paymentTypeAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(paymentType.Name == "chase");
            }
        }

        [Fact]
        public async Task Modify_A_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                string newName = "american express";
                PaymentType modifiedPaymentType = new PaymentType()
                {
                    Name = newName,
                    AccountNumber = "38264 - 49271",
                    CustomerId = 3
                };
                var paymentTypeAsJSON = JsonConvert.SerializeObject(modifiedPaymentType);

                var response = await client.PutAsync(
                    "/api/paymentTypes/3",
                    new StringContent(paymentTypeAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getPaymentType = await client.GetAsync("/api/paymentTypes/3");
                getPaymentType.EnsureSuccessStatusCode();

                string getPaymentTypeBody = await getPaymentType.Content.ReadAsStringAsync();
                PaymentType newPaymentType = JsonConvert.DeserializeObject<PaymentType>(getPaymentTypeBody);

                Assert.Equal(HttpStatusCode.OK, getPaymentType.StatusCode);
                Assert.Equal(newName, newPaymentType.Name);
            }
        }

        [Fact]
        public async Task Test_Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                PaymentType newPaymentType = new PaymentType()
                {
                    Name = "mastercard",
                    AccountNumber = "37164-03927",
                    CustomerId = 2
                };
                var paymentTypeAsJSON = JsonConvert.SerializeObject(newPaymentType);

                var postResponse = await client.PostAsync(
                    "/api/paymentTypes",
                    new StringContent(paymentTypeAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await postResponse.Content.ReadAsStringAsync();

                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                /*
                    ACT
                */

                var deleteResponse = await client.DeleteAsync($"/api/paymentTypes/{paymentType.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }
    }
}
