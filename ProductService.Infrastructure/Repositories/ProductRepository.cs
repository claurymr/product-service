using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Domain;

namespace ProductService.Infrastructure.Repositories;
public class ProductRepository : IProductRepository
{
    public Task<Guid> CreateProductAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteProductAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductResponse>> GetProductByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductResponse>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductResponse>> GetProductsByCategory(string category)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateProductAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}