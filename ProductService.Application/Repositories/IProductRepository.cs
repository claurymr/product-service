using ProductService.Application.Contracts;
using ProductService.Domain;

namespace ProductService.Application.Repositories;
public interface IProductRepository
{
    Task<Guid> CreateProductAsync(Product product);
    Task<Guid> DeleteProductAsync(Guid id);
    Task<IEnumerable<ProductResponse>> GetProductsAsync();
    Task<IEnumerable<ProductResponse>> GetProductsByCategory(string category);
    Task<ProductResponse> GetProductByIdAsync(Guid id);
    Task<Guid> UpdateProductAsync(Guid id);
}