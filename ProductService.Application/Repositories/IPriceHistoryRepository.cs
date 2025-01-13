using ProductService.Domain;

namespace ProductService.Application.Repositories;
public interface IPriceHistoryRepository
{
    Task<IEnumerable<PriceHistory>> GetPriceHistoryByProductIdAsync(Guid id);
}