using ProductService.Application.ProductPriceHistories.GetPriceHistories;

namespace ProductService.Application.Repositories;
public interface IPriceHistoryRepository
{
    Task<IEnumerable<PriceHistoryQueryResponse>> GetPriceHistoryByProductIdAsync(Guid id);
}