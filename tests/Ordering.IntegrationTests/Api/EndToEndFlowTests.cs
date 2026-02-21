using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Ordering.IntegrationTests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Ordering.IntegrationTests.Api;

public sealed class EndToEndFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;

    public EndToEndFlowTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _httpClient = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task FullFlow_ShouldRegisterLoginCreateCustomerCreateOrderAndGetOrderById()
    {
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
        var username = $"flow-user-{uniqueSuffix}";
        var password = "StrongPass123";
        var email = $"{username}@ordering.local";

        var registerRequest = new
        {
            username,
            email,
            password
        };

        var registerResponse = await SendPostAndLogAsync("REGISTER", "/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<ApiSuccessResponse<RegisterResponse>>();
        registerPayload.Should().NotBeNull();
        registerPayload!.Data.Username.Should().Be(username);

        var loginResponse = await SendPostAndLogAsync("LOGIN_NEW_USER", "/api/auth/login", new { username, password });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<ApiSuccessResponse<AuthTokensResponse>>();
        loginPayload.Should().NotBeNull();
        loginPayload!.Data.AccessToken.Should().NotBeNullOrWhiteSpace();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Data.AccessToken);

        var createCustomerRequest = new
        {
            fullName = "Flow Test Customer",
            email = $"customer-{uniqueSuffix}@example.com",
            documentNumber = $"DOC-{uniqueSuffix}",
            phoneNumber = "+5511999990000",
            customerType = "individual",
            birthDate = "1992-02-10",
            street = "Test Street 100",
            city = "Sao Paulo",
            state = "SP",
            postalCode = "01000-000",
            country = "Brazil",
            notes = "Created by integration flow test"
        };

        var createCustomerResponse = await SendPostAndLogAsync("CREATE_CUSTOMER", "/api/customers", createCustomerRequest);
        createCustomerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var customerPayload = await createCustomerResponse.Content.ReadFromJsonAsync<ApiSuccessResponse<CreateCustomerResponse>>();
        customerPayload.Should().NotBeNull();
        var customerId = customerPayload!.Data.CustomerId;

        var createOrderRequest = new
        {
            customerId,
            items = new[]
            {
                new
                {
                    productId = Guid.NewGuid(),
                    quantity = 2,
                    unitPrice = 45.5m
                }
            }
        };

        var createOrderResponse = await SendPostAndLogAsync("CREATE_ORDER", "/api/orders", createOrderRequest);
        createOrderResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var orderPayload = await createOrderResponse.Content.ReadFromJsonAsync<ApiSuccessResponse<CreateOrderResponse>>();
        orderPayload.Should().NotBeNull();
        var orderId = orderPayload!.Data.OrderId;

        var getOrderResponse = await SendGetAndLogAsync("GET_ORDER_BY_ID", $"/api/orders/{orderId}");
        getOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getOrderPayload = await getOrderResponse.Content.ReadFromJsonAsync<ApiSuccessResponse<OrderDetailsResponse>>();
        getOrderPayload.Should().NotBeNull();
        getOrderPayload!.Data.OrderId.Should().Be(orderId);
        getOrderPayload.Data.CustomerId.Should().Be(customerId);
    }

    private async Task<HttpResponseMessage> SendPostAndLogAsync(string step, string url, object payload)
    {
        _output.WriteLine($"========== {step} ==========");
        _output.WriteLine($"Request: POST {url}");
        _output.WriteLine($"Request Headers: {FormatAuthHeader()}");
        _output.WriteLine($"Request Payload:\n{ToPrettyJson(payload)}");

        var response = await _httpClient.PostAsJsonAsync(url, payload);
        await LogResponseAsync(step, response);
        return response;
    }

    private async Task<HttpResponseMessage> SendGetAndLogAsync(string step, string url)
    {
        _output.WriteLine($"========== {step} ==========");
        _output.WriteLine($"Request: GET {url}");
        _output.WriteLine($"Request Headers: {FormatAuthHeader()}");

        var response = await _httpClient.GetAsync(url);
        await LogResponseAsync(step, response);
        return response;
    }

    private async Task LogResponseAsync(string step, HttpResponseMessage response)
    {
        var rawBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response Headers: {string.Join(", ", response.Headers.Select(header => $"{header.Key}={string.Join("|", header.Value)}"))}");
        _output.WriteLine($"Response Content-Type: {response.Content.Headers.ContentType}");

        if (string.IsNullOrWhiteSpace(rawBody))
        {
            _output.WriteLine("Response Body: <empty>");
            return;
        }

        try
        {
            using var json = JsonDocument.Parse(rawBody);
            var pretty = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
            _output.WriteLine($"Response Body:\n{pretty}");
        }
        catch
        {
            _output.WriteLine($"Response Body:\n{rawBody}");
        }
    }

    private string FormatAuthHeader()
    {
        var auth = _httpClient.DefaultRequestHeaders.Authorization;
        if (auth is null || string.IsNullOrWhiteSpace(auth.Parameter))
        {
            return "Authorization=<none>";
        }

        var token = auth.Parameter;
        var preview = token.Length <= 16 ? token : $"{token[..8]}...{token[^8..]}";
        return $"Authorization={auth.Scheme} {preview}";
    }

    private static string ToPrettyJson(object payload)
    {
        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }
}
