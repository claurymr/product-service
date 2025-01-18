namespace Shared.Contracts.Events;
/// <summary>
/// Represents an event that is triggered when a product is created.
/// </summary>
public record ProductCreatedEvent
{
    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
}
