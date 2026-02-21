using Microsoft.AspNetCore.Identity;

namespace Ordering.Api.Security;

public sealed class IdentitySeedHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentitySeedHostedService> _logger;

    public IdentitySeedHostedService(IServiceProvider serviceProvider, ILogger<IdentitySeedHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var roles = new[] { "admin", "sales", "viewer", "catalog-manager" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(userManager, "admin", "admin@ordering.local", "Admin123!", ["admin", "sales", "viewer", "catalog-manager"]);
        await EnsureUserAsync(userManager, "sales", "sales@ordering.local", "Sales123!", ["sales", "viewer"]);

        _logger.LogInformation("Identity seed completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task EnsureUserAsync(
        UserManager<IdentityUser> userManager,
        string username,
        string email,
        string password,
        IReadOnlyCollection<string> roles)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to create identity user '{username}': {errors}");
            }
        }

        var existingRoles = await userManager.GetRolesAsync(user);
        var missingRoles = roles.Except(existingRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        if (missingRoles.Length > 0)
        {
            var addRolesResult = await userManager.AddToRolesAsync(user, missingRoles);
            if (!addRolesResult.Succeeded)
            {
                var errors = string.Join(", ", addRolesResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to assign roles to '{username}': {errors}");
            }
        }
    }
}
