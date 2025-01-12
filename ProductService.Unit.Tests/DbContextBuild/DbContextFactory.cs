using ProductService.Infrastructure.Data;

namespace ProductService.Unit.Tests.DbContextBuild;
public class DbContextFactory
{
    public static ProductServiceDbContext CreateInMemoryDbContext()
    {
        return new TestDbContext().Context;
    }
}