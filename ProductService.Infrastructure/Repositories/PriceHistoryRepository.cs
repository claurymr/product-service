using Microsoft.EntityFrameworkCore;
using ProductService.Application.Repositories;
using ProductService.Domain;
using ProductService.Domain.Enums;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;
public sealed class PriceHistoryRepository(ProductServiceDbContext dbContext) : IPriceHistoryRepository
{
    public async Task CreatePriceHistoryByProductIdAsync(Guid id, decimal oldPrice, decimal newPrice, ActionType actionType)
    {
        dbContext.PriceHistories.Add(new PriceHistory
        {
            ProductId = id,
            OldPrice = oldPrice,
            NewPrice = newPrice,
            Action = actionType,
            Timestamp = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryByProductIdAsync(Guid id)
    {
        return await dbContext.PriceHistories
                        .Where(ph => ph.ProductId == id)
                        .Include(ph => ph.Product)
                        .ToListAsync();
    }
}