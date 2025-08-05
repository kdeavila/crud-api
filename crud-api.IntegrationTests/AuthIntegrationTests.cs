using System.Net.Http.Json;
using crud_api.DTOs.Auth;
using System.Net;

namespace crud_api.IntegrationTests;

public class AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsNotFound()
    {
        var loginDto = new UserLoginDto
        {
            Email = "invalid@example.com",
            Password = "invalidPassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithJWT()
    {
        // Arrange
        
    }
}
