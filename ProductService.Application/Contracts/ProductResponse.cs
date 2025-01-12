namespace ProductService.Application.Contracts;
public record ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Currency { get; set; }
    public string Category { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
}
