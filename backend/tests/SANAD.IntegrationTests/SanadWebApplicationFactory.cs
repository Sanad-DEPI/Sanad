using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SANAD.Infrastructure.Persistence;

namespace SANAD.IntegrationTests;

/// <summary>
/// Isolated in-memory host for endpoint, authentication, and authorization tests.
/// The real application still uses SQL Server.
/// </summary>
public sealed class SanadWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"SANAD_Tests_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting(
            "ConnectionStrings:Default",
            "Server=unused;Database=SANAD.Tests;Trusted_Connection=True;TrustServerCertificate=True");
        builder.UseSetting("Jwt:Secret", "integration-tests-secret-with-at-least-32-characters");
        builder.UseSetting("Jwt:Issuer", "SANAD.Tests");
        builder.UseSetting("Jwt:Audience", "SANAD.Tests.Client");
        builder.UseSetting("Jwt:ExpiryMinutes", "60");

        builder.ConfigureTestServices(services =>
        {
            var descriptorsToRemove = services
                .Where(service =>
                    service.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    (service.ServiceType.IsGenericType &&
                     service.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>) &&
                     service.ServiceType.GenericTypeArguments[0] == typeof(AppDbContext)))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }
}
