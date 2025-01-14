using ProductService.Domain;

namespace ProductService.Application.Repositories;
public interface IProductRepository
{
    Task<Guid> CreateProductAsync(Product product);
    Task<Guid> DeleteProductAsync(Guid id);
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<Product> GetProductByIdAsync(Guid id);
    Task<(Guid ProductId, decimal OldPrice)> UpdateProductAsync(Guid id, Product product);
}