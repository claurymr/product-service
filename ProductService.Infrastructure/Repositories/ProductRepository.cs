using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Domain;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;
public class ProductRepository(ProductServiceDbContext dbContext) : IProductRepository
{
    private readonly ProductServiceDbContext _dbContext = dbContext;

    public Task<Guid> CreateProductAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteProductAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ProductResponse> GetProductByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductResponse>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(string category)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateProductAsync(Guid id, Product product)
    {
        throw new NotImplementedException();
    }
}