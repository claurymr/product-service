using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IHost"/> interface.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Applies any pending migrations for the context to the database and ensures that the database is created.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance to apply migrations to.</param>
    /// <returns>The <see cref="IHost"/> instance with migrations applied.</returns>
    /// <exception cref="Exception">Thrown if an error occurs while migrating the database.</exception>
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductServiceDbContext>();

        try
        {
            // Apply pending migrations
            dbContext.Database.Migrate();

            // Ensure database creation
            dbContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProductServiceDbContext>>();
            logger.LogError(ex, "An error occurred while migrating.");
        }

        return host;
    }
}