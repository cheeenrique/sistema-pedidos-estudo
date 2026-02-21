using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Contracts.Auth;

public sealed record LoginRequest(
    [property: Required] string Username,
    [property: Required] string Password);

public sealed record RegisterRequest(
    [property: Required] string Username,
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresInSeconds);

public sealed record RegisterResponse(
    string UserId,
    string Username,
    string Email);

public sealed record RefreshTokenRequest([property: Required] string RefreshToken);

public sealed record RevokeTokenRequest(string? RefreshToken);
