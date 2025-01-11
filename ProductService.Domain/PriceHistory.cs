using ProductService.Domain.Enums;

namespace ProductService.Domain;

public class PriceHistory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public double OldPrice { get; set; }
    public double NewPrice { get; set; }
    public ActionType Action { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Product Product { get; set; }
}
