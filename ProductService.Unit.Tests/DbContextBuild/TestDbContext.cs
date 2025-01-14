using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Data;
using Microsoft.Data.Sqlite;

namespace ProductService.Unit.Tests.DbContextBuild;

public class TestDbContext : IDisposable
{
    private readonly SqliteConnection _connection;
    public ProductServiceDbContext Context { get; }

    public TestDbContext()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<ProductServiceDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ProductServiceDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}