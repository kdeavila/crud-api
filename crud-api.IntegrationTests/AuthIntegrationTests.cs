using System.Net;
using System.Net.Http.Json;
using crud_api.DTOs.Auth;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace crud_api.IntegrationTests;

public class AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var loginDto = new UserLoginDto
        {
            Email = "invalidEmail",
            Password = "invalidPassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}