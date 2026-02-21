using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Contracts.Auth;

public sealed class LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed class RegisterRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresInSeconds);

public sealed record RegisterResponse(
    string UserId,
    string Username,
    string Email);

public sealed class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}

public sealed class RevokeTokenRequest
{
    public string? RefreshToken { get; init; }
}
