namespace Shared.Contracts.Events;
public record ProductCreatedEvent
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
}
