namespace ProductService.Application.Products.CreateProducts;
public record ProductCreatedEvent
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
}
