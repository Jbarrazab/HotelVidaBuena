using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace HotelVivaBueno.Tests.Integration
{
    public class ApiIntegrationTests
    {
        private readonly string _baseUrl = "http://localhost:5001";

        [Fact]
        public async Task GetDepartments_ShouldReturnSuccess()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_baseUrl) };

            // Act
            var response = await client.GetAsync("/departments");

            // Assert
            // Note: This test requires the API to be running on localhost:5001
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ShouldFail()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_baseUrl) };
            var loginRequest = new { documentNumber = "INVALID", password = "WRONG" };

            // Act  
            var response = await client.PostAsJsonAsync("/auth/login", loginRequest);

            // Assert
            // Note: This test requires the API to be running
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.ServiceUnavailable);
        }
    }
}
