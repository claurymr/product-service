namespace ProductService.Application.Contracts;
/// <summary>
/// Represents a response containing product details.
/// </summary>
public record ProductResponse
{
    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description of the product.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the price of the product.
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Gets the currency of the product price.
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the category of the product.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Gets the SKU (Stock Keeping Unit) of the product.
    /// </summary>
    public string Sku { get; init; } = string.Empty;
}
