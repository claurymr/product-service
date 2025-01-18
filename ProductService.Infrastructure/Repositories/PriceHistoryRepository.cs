using Microsoft.EntityFrameworkCore;
using ProductService.Application.Repositories;
using ProductService.Domain;
using ProductService.Domain.Enums;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;
/// <summary>
/// Repository for managing price history records in the database.
/// </summary>
/// <param name="dbContext">The database context used to access the price history records.</param>
public sealed class PriceHistoryRepository(ProductServiceDbContext dbContext) : IPriceHistoryRepository
{
    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryByProductIdAsync(Guid id)
    {
        return await dbContext.PriceHistories
                        .Where(ph => ph.ProductId == id)
                        .Include(ph => ph.Product)
                        .ToListAsync();
    }
}