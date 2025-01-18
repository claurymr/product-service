namespace Shared.Contracts.Events;
/// <summary>
/// Represents an event that is triggered when a product is deleted.
/// </summary>
public class ProductDeletedEvent
{
    /// <summary>
    /// Gets or sets the unique identifier of the deleted product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the deleted product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
}