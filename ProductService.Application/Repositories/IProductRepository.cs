using ProductService.Domain;

namespace ProductService.Application.Repositories;

/// <summary>
/// Interface for product repository operations.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Creates a new product asynchronously.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the created product.</returns>
    Task<Guid> CreateProductAsync(Product product);

    /// <summary>
    /// Deletes a product asynchronously.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the deleted product.</returns>
    Task<Guid> DeleteProductAsync(Guid id);

    /// <summary>
    /// Gets all products asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of products.</returns>
    Task<IEnumerable<Product>> GetProductsAsync();

    /// <summary>
    /// Gets products by category asynchronously.
    /// </summary>
    /// <param name="category">The category of the products to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of products in the specified category.</returns>
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);

    /// <summary>
    /// Gets a product by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the product with the specified ID.</returns>
    Task<Product> GetProductByIdAsync(Guid id);

    /// <summary>
    /// Updates a product asynchronously.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="product">The updated product information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the product ID and the old price of the product.</returns>
    Task<(Guid ProductId, decimal OldPrice)> UpdateProductAsync(Guid id, Product product);
}