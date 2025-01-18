namespace Shared.Contracts.Events;
public class ProductUpdatedEvent
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
}