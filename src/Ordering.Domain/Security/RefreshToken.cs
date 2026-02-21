using Ordering.Domain.Common;

namespace Ordering.Domain.Security;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    private RefreshToken(
        string userId,
        string tokenHash,
        DateTime expiresAtUtc,
        string? createdByIp,
        string? createdByUserAgent)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
        CreatedByIp = createdByIp;
        CreatedByUserAgent = createdByUserAgent;
    }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? CreatedByIp { get; private set; }
    public string? CreatedByUserAgent { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? RevokedByUserAgent { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;

    public static RefreshToken Create(
        string userId,
        string tokenHash,
        DateTime expiresAtUtc,
        string? createdByIp = null,
        string? createdByUserAgent = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash is required.", nameof(tokenHash));
        }

        if (expiresAtUtc <= DateTime.UtcNow)
        {
            throw new ArgumentException("Refresh token expiration must be in the future.", nameof(expiresAtUtc));
        }

        var normalizedTokenHash = NormalizeTokenHash(tokenHash);

        return new RefreshToken(
            userId.Trim(),
            normalizedTokenHash,
            expiresAtUtc,
            NormalizeMetadata(createdByIp),
            NormalizeMetadata(createdByUserAgent));
    }

    public bool Revoke(
        string? replacedByTokenHash = null,
        string? revokedByIp = null,
        string? revokedByUserAgent = null)
    {
        if (RevokedAtUtc.HasValue)
        {
            return false;
        }

        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenHash = string.IsNullOrWhiteSpace(replacedByTokenHash) ? null : NormalizeTokenHash(replacedByTokenHash);
        RevokedByIp = NormalizeMetadata(revokedByIp);
        RevokedByUserAgent = NormalizeMetadata(revokedByUserAgent);
        return true;
    }

    private static string NormalizeTokenHash(string tokenHash)
    {
        return tokenHash.Trim().ToUpperInvariant();
    }

    private static string? NormalizeMetadata(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
