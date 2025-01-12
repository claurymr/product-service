using ProductService.Application.Enums;

namespace ProductService.Application.Contracts;
public record PriceHistoryResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public string? Currency { get; set; }
    public ActionType Action { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}