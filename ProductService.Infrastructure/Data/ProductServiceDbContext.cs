using Microsoft.EntityFrameworkCore;
using ProductService.Domain;

namespace ProductService.Infrastructure.Data;

/// <summary>
/// Represents the database context for the Product Service.
/// </summary>
/// <param name="options">The options to be used by the DbContext.</param>
public class ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductServiceDbContext).Assembly);
    }
}
