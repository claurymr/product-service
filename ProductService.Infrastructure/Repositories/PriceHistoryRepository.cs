using ProductService.Application.Contracts;
using ProductService.Application.Repositories;

namespace ProductService.Infrastructure.Repositories;
public class PriceHistoryRepository : IPriceHistoryRepository
{
    public Task<IEnumerable<PriceHistoryResponse>> GetPriceHistoryByProductIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}