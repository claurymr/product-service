using ProductService.Application.ProductPriceHistories.GetPriceHistories;
using ProductService.Application.Repositories;

namespace ProductService.Infrastructure.Repositories;
public class PriceHistoryRepository : IPriceHistoryRepository
{
    public Task<IEnumerable<PriceHistoryQueryResponse>> GetPriceHistoryByProductIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}