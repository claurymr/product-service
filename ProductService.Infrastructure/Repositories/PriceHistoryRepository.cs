using Microsoft.EntityFrameworkCore;
using ProductService.Application.Repositories;
using ProductService.Domain;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;
public sealed class PriceHistoryRepository(ProductServiceDbContext dbContext) : IPriceHistoryRepository
{

    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryByProductIdAsync(Guid id)
    {
        return await dbContext.PriceHistories
                        .Where(ph => ph.ProductId == id)
                        .Include(ph => ph.Product)
                        .ToListAsync();
    }
}