namespace ProductService.Application.Contracts;
public record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Currency { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
}
