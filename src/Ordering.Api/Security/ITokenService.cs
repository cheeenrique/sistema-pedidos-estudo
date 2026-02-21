namespace Ordering.Api.Security;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string username, IReadOnlyCollection<string> roles);
    string GenerateRefreshToken();
    string HashToken(string rawToken);
}
