using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure;
public class DesignTimeContextFactory : IDesignTimeDbContextFactory<ProductServiceDbContext>
{
    public ProductServiceDbContext CreateDbContext(string[] args)
    {
        var apiProjectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "ProductService.Api");
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

        var connectionString = configuration.GetConnectionString("ProductServiceConnection");
        var optionsBuilder = new DbContextOptionsBuilder<ProductServiceDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new ProductServiceDbContext(optionsBuilder.Options);
    }
}