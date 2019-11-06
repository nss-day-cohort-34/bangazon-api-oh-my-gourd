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
    public class TestProductTypes
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/producttypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_A_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/producttypes/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductType>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(product.Id == 1);
            }
        }

        [Fact]
        public async Task Create_A_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                ProductType newProductType = new ProductType()
                {
                    Name = "Pants"
                };
                var productTypeAsJSON = JsonConvert.SerializeObject(newProductType);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/producttypes",
                    new StringContent(productTypeAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(productType.Name == "Pants");
            }
        }

        [Fact]
        public async Task Modify_A_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Random rnd = new Random();
                int newInt = rnd.Next(1, 100);
                ProductType modifiedProductType = new ProductType()
                {
                    Name = $"Test: {newInt}"
                };
                var productAsJSON = JsonConvert.SerializeObject(modifiedProductType);

                var response = await client.PutAsync(
                    "/api/producttypes/3",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getProductType = await client.GetAsync("/api/producttypes/3");
                getProductType.EnsureSuccessStatusCode();

                string getProductTypeBody = await getProductType.Content.ReadAsStringAsync();
                ProductType newProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);

                Assert.Equal(HttpStatusCode.OK, getProductType.StatusCode);
                Assert.Equal($"Test: {newInt}", newProductType.Name);
            }
        }

        [Fact]
        public async Task Test_Delete_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                ProductType newProductType = new ProductType()
                {
                    Name = "DELETE TEST"
                };
                var productAsJSON = JsonConvert.SerializeObject(newProductType);

                var postResponse = await client.PostAsync(
                    "/api/producttypes",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await postResponse.Content.ReadAsStringAsync();

                var product = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ACT
                */



                var deleteResponse = await client.DeleteAsync($"/api/producttypes/{product.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }

    }
}
