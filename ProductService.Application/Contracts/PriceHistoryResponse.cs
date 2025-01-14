using ProductService.Application.Enums;

namespace ProductService.Application.Contracts;
public record PriceHistoryResponse
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public decimal OldPrice { get; init; }
    public decimal NewPrice { get; init; }
    public string? Currency { get; init; }
    public ActionType Action { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}