using System.Net;
using FluentAssertions;
using Ordering.IntegrationTests.Infrastructure;
using Xunit;

namespace Ordering.IntegrationTests.Api;

public sealed class OrdersEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public OrdersEndpointTests(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrders_ShouldReturnUnauthorized_WhenRequestHasNoToken()
    {
        var response = await _httpClient.GetAsync("/api/orders");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
