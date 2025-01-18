using ProductService.Application.Enums;

namespace ProductService.Application.Contracts;
/// <summary>
/// Represents the response containing the price history of a product.
/// </summary>
public record PriceHistoryResponse
{
    /// <summary>
    /// Gets the unique identifier of the price history record.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the SKU (Stock Keeping Unit) of the product.
    /// </summary>
    public string ProductSku { get; init; } = string.Empty;

    /// <summary>
    /// Gets the old price of the product before the change.
    /// </summary>
    public decimal OldPrice { get; init; }

    /// <summary>
    /// Gets the new price of the product after the change.
    /// </summary>
    public decimal NewPrice { get; init; }

    /// <summary>
    /// Gets the currency of the price.
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the action type that triggered the price change.
    /// </summary>
    public ActionType Action { get; init; }

    /// <summary>
    /// Gets the timestamp when the price change occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }
}