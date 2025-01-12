using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;
public sealed class PriceHistoryRepository(ProductServiceDbContext dbContext) : IPriceHistoryRepository
{

    public Task<IEnumerable<PriceHistoryResponse>> GetPriceHistoryByProductIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}