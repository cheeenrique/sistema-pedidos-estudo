namespace Ordering.IntegrationTests.Api;

public sealed record ApiSuccessResponse<T>(bool Success, string Message, T Data, string TraceId);

public sealed record AuthTokensResponse(string AccessToken, string RefreshToken, string TokenType, int ExpiresInSeconds);

public sealed record RegisterResponse(string UserId, string Username, string Email);

public sealed record CreateCustomerResponse(Guid CustomerId);

public sealed record CreateOrderResponse(Guid OrderId, decimal TotalAmount, string Status);

public sealed record OrderDetailsResponse(Guid OrderId, Guid CustomerId, decimal TotalAmount, string Status);
