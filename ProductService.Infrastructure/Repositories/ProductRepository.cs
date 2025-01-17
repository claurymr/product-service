using Microsoft.EntityFrameworkCore;
using ProductService.Application.Repositories;
using ProductService.Domain;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;
public class ProductRepository(ProductServiceDbContext dbContext) : IProductRepository
{
    public async Task<Guid> CreateProductAsync(Product product)
    {
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
        return product.Id;
    }

    public async Task<Guid> DeleteProductAsync(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return Guid.Empty;
        }
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        return product.Id;
    }

    public async Task<Product> GetProductByIdAsync(Guid id) => (await dbContext.Products.FindAsync(id))!;

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await dbContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await dbContext.Products.Where(p => p.Category == category).ToListAsync();
    }

    public async Task<(Guid ProductId, decimal OldPrice)> UpdateProductAsync(Guid id, Product product)
    {
        var existingProduct = await dbContext.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return (Guid.Empty, default(decimal));
        }

        var oldPrice = existingProduct.Price;
        existingProduct.Name = product.Name;
        existingProduct.Category = product.Category;
        existingProduct.Price = product.Price;
        existingProduct.Sku = product.Sku;
        await dbContext.SaveChangesAsync();

        return (existingProduct.Id, oldPrice);
    }
}