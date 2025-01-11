using ProductService.Application.Products.GetProducts;
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

    public Task<IEnumerable<ProductQueryResponse>> GetProductByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductQueryResponse>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductQueryResponse>> GetProductsByCategory(string category)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateProductAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}