using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Api.Infrastructure;

public sealed class DatabaseInitializationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DatabaseInitializationHostedService> _logger;

    public DatabaseInitializationHostedService(
        IServiceProvider serviceProvider,
        IWebHostEnvironment environment,
        ILogger<DatabaseInitializationHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _environment = environment;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!(_environment.IsDevelopment() || _environment.IsEnvironment("Testing")))
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Database schema ensured for {Environment}.", _environment.EnvironmentName);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
