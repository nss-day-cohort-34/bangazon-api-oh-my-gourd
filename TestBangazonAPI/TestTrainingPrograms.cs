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
    public class TestTrainingPrograms
    {
        [Fact]
        public async Task Test_Get_All_TrainingPrograms()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/trainingPrograms");


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_A_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/trainingprograms/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingProgram[0].Id == 1);
            }
        }

        [Fact]
        public async Task Create_A_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                TrainingProgram newTrainingProgram = new TrainingProgram()
                {
                    Name = "test post",
                    MaxAttendees = 70,
                    StartDate = new DateTime(2020,3,2),
                    EndDate = new DateTime(2020,4,2)
                };
                var trainingProgramAsJSON = JsonConvert.SerializeObject(newTrainingProgram);


                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/trainingprograms",
                    new StringContent(trainingProgramAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);
                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(trainingProgram.Name == "test post");
            }
        }

        [Fact]
        public async Task Modify_A_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {

                var getAllResponse = await client.GetAsync("/api/trainingPrograms");


                string getAllResponseBody = await getAllResponse.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(getAllResponseBody);
                /*
                    PUT section
                */
                Random rnd = new Random();
                int random = rnd.Next(1, 1000);
                string newName = $"test put {random}";
                TrainingProgram modifiedTrainingProgram = new TrainingProgram()
                {
                    Name = "test post",
                    MaxAttendees = 70,
                    StartDate = new DateTime(2020, 3, 2),
                    EndDate = new DateTime(2020, 4, 2)
                };
                var trainingProgramAsJSON = JsonConvert.SerializeObject(modifiedTrainingProgram);

                var response = await client.PutAsync(
                    $"/api/trainingPrograms/{trainingPrograms[0].Id}",
                    new StringContent(trainingProgramAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */

                var getTrainingProgram = await client.GetAsync($"/api/trainingPrograms/{trainingPrograms[0].Id}");
                getTrainingProgram.EnsureSuccessStatusCode();

                string getTrainingProgramBody = await getTrainingProgram.Content.ReadAsStringAsync();
                var newTrainingProgram = JsonConvert.DeserializeObject<List<TrainingProgram>>(getTrainingProgramBody);

                Assert.Equal(HttpStatusCode.OK, getTrainingProgram.StatusCode);
                Assert.Equal(modifiedTrainingProgram.Name, newTrainingProgram[0].Name);
            }
        }
        [Fact]
        public async Task Test_Delete_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                TrainingProgram newTrainingProgram = new TrainingProgram()
                {
                    Name = "test post",
                    MaxAttendees = 70,
                    StartDate = new DateTime(2020, 3, 2),
                    EndDate = new DateTime(2020, 4, 2)
                };
                var trainingProgramAsJSON = JsonConvert.SerializeObject(newTrainingProgram);

                var postResponse = await client.PostAsync(
                    "/api/trainingPrograms",
                    new StringContent(trainingProgramAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await postResponse.Content.ReadAsStringAsync();

                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                /*
                    ACT
                */



                var deleteResponse = await client.DeleteAsync($"/api/trainingPrograms/{trainingProgram.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }
    }
}
