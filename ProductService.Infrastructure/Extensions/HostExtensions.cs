using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Extensions;
public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductServiceDbContext>();

        try
        {
            // Apply pending migrations
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProductServiceDbContext>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }

        return host;
    }
}