using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Infrastructure.Persistence;

namespace Ordering.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"ordering-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(service =>
                service.ServiceType == typeof(DbContextOptions<OrderingDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<OrderingDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });

        builder.UseEnvironment("Testing");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        dbContext.Database.EnsureCreated();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        SeedIdentityAsync(roleManager, userManager).GetAwaiter().GetResult();

        return host;
    }

    private static async Task SeedIdentityAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
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
    }

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
                return;
            }
        }

        var existingRoles = await userManager.GetRolesAsync(user);
        var missingRoles = roles.Except(existingRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        if (missingRoles.Length > 0)
        {
            await userManager.AddToRolesAsync(user, missingRoles);
        }
    }
}
