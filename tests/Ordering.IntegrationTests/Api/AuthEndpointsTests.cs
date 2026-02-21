using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Ordering.IntegrationTests.Infrastructure;

namespace Ordering.IntegrationTests.Api;

public sealed class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnAccessAndRefreshToken_WhenCredentialsAreValid()
    {
        var request = new
        {
            username = "admin",
            password = "Admin123!"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<ApiSuccessResponse<AuthTokensResponse>>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Data.AccessToken.Should().NotBeNullOrWhiteSpace();
        payload.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var request = new
        {
            username = "admin",
            password = "wrong-password"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

public sealed record ApiSuccessResponse<T>(bool Success, string Message, T Data, string TraceId);

public sealed record AuthTokensResponse(string AccessToken, string RefreshToken, string TokenType, int ExpiresInSeconds);
