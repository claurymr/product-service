using ProductService.Application.Products.GetProducts;
using ProductService.Domain;

namespace ProductService.Application.Repositories;
public interface IProductRepository
{
    Task<Guid> CreateProductAsync(Product product);
    Task<Guid> DeleteProductAsync(Guid id);
    Task<IEnumerable<ProductQueryResponse>> GetProductsAsync();
    Task<IEnumerable<ProductQueryResponse>> GetProductsByCategory(string category);
    Task<IEnumerable<ProductQueryResponse>> GetProductByIdAsync(Guid id);
    Task<Guid> UpdateProductAsync(Guid id);
}