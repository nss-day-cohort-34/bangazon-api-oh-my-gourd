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
    public class TestProducts
    {

        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_A_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(product.Id == 1);
            }
        }

        [Fact]
        public async Task Create_A_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                Product newProduct = new Product()
                {
                    ProductTypeId = 1,
                    SellerId = 2,
                    Price = 5467,
                    Title = "Broom",
                    Description = "Sweeps",
                    Quantity = 4
                };
                var productAsJSON = JsonConvert.SerializeObject(newProduct);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/products",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(product.Description == "Sweeps");
            }
        }

        [Fact]
        public async Task Modify_A_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Random rnd = new Random();
                int newPrice = rnd.Next(1, 1000);
                Product modifiedProduct = new Product()
                {
                    ProductTypeId = 1,
                    SellerId = 2,
                    Price = newPrice,
                    Title = "Broom",
                    Description = "Sweeps",
                    Quantity = 4
                };
                var productAsJSON = JsonConvert.SerializeObject(modifiedProduct);

                var response = await client.PutAsync(
                    "/api/products/5",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getProduct = await client.GetAsync("/api/products/5");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.OK, getProduct.StatusCode);
                Assert.Equal(newPrice, newProduct.Price);
            }
        }

        [Fact]
        public async Task Test_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Product newProduct = new Product()
                {
                    ProductTypeId = 1,
                    SellerId = 2,
                    Price = 1,
                    Title = "DELETE_TEST",
                    Description = "Test To Delete",
                    Quantity = 1
                };
                var productAsJSON = JsonConvert.SerializeObject(newProduct);

                var postResponse = await client.PostAsync(
                    "/api/products",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await postResponse.Content.ReadAsStringAsync();

                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ACT
                */



                var deleteResponse = await client.DeleteAsync($"/api/products/{product.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }
    }
}
