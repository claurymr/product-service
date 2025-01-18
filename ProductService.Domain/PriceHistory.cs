using ProductService.Domain.Enums;

namespace ProductService.Domain;

/// <summary>
/// Represents the history of price changes for a product.
/// </summary>
public class PriceHistory
{
    /// <summary>
    /// Gets or sets the unique identifier for the price history record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the product associated with the price change.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the old price of the product before the change.
    /// </summary>
    public decimal OldPrice { get; set; }

    /// <summary>
    /// Gets or sets the new price of the product after the change.
    /// </summary>
    public decimal NewPrice { get; set; }

    /// <summary>
    /// Gets or sets the type of action that caused the price change.
    /// </summary>
    public ActionType Action { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the price change occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the product associated with the price change.
    /// </summary>
    public Product Product { get; set; }
}
