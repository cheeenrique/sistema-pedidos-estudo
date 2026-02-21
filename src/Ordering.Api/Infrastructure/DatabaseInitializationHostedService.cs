using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Api.Infrastructure;

public sealed class DatabaseInitializationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DatabaseStartupOptions _startupOptions;
    private readonly ILogger<DatabaseInitializationHostedService> _logger;

    public DatabaseInitializationHostedService(
        IServiceProvider serviceProvider,
        IOptions<DatabaseStartupOptions> startupOptions,
        ILogger<DatabaseInitializationHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _startupOptions = startupOptions.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_startupOptions.RunMigrationsOnStartup)
        {
            _logger.LogInformation("Database migration on startup is disabled.");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        if (dbContext.Database.IsRelational())
        {
            var allMigrations = dbContext.Database.GetMigrations();
            if (allMigrations.Any())
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migrations applied successfully.");
                return;
            }

            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            _logger.LogInformation("No migrations found. Database schema created with EnsureCreated for development.");
            return;
        }

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Database schema ensured for non-relational provider.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
