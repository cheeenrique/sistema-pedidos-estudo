namespace Ordering.Api.Infrastructure;

public sealed class DatabaseStartupOptions
{
    public const string SectionName = "Database";

    public bool RunMigrationsOnStartup { get; init; } = false;
}
