namespace Shared.Contracts.Events;
public class ProductDeletedEvent
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
}