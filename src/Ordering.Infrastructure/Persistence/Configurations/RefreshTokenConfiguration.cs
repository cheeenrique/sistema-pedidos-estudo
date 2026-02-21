using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Security;

namespace Ordering.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(token => token.Id);
        builder.Property(token => token.UserId).HasMaxLength(64).IsRequired();
        builder.Property(token => token.TokenHash).HasMaxLength(256).IsRequired();
        builder.Property(token => token.ExpiresAtUtc).IsRequired();
        builder.Property(token => token.CreatedAtUtc).IsRequired();
        builder.Property(token => token.RevokedAtUtc);
        builder.Property(token => token.ReplacedByTokenHash).HasMaxLength(256);

        builder.Ignore(token => token.DomainEvents);
        builder.Ignore(token => token.IsActive);

        builder.HasIndex(token => token.TokenHash).IsUnique();
        builder.HasIndex(token => token.UserId);
    }
}
