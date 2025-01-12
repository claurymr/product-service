using ProductService.Application.Contracts;

namespace ProductService.Application.Repositories;
public interface IPriceHistoryRepository
{
    Task<IEnumerable<PriceHistoryResponse>> GetPriceHistoryByProductIdAsync(Guid id);
}