using ProductService.Domain;
using ProductService.Domain.Enums;

namespace ProductService.Application.Repositories;
/// <summary>
/// Interface for managing price history related operations.
/// </summary>
public interface IPriceHistoryRepository
{
    /// <summary>
    /// Retrieves the price history for a specific product by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="PriceHistory"/>.</returns>
    Task<IEnumerable<PriceHistory>> GetPriceHistoryByProductIdAsync(Guid id);

    /// <summary>
    /// Creates a new price history record for a specific product.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="oldPrice">The previous price of the product.</param>
    /// <param name="newPrice">The new price of the product.</param>
    /// <param name="actionType">The type of action that triggered the price change.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreatePriceHistoryByProductIdAsync(Guid id, decimal oldPrice, decimal newPrice, ActionType actionType);
}