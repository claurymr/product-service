using ProductService.Domain;
using ProductService.Domain.Enums;

namespace ProductService.Application.Repositories;
public interface IPriceHistoryRepository
{
    Task<IEnumerable<PriceHistory>> GetPriceHistoryByProductIdAsync(Guid id);
    Task CreatePriceHistoryByProductIdAsync(Guid id, decimal oldPrice, decimal newPrice, ActionType actionType);
}