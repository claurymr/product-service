namespace ProductService.Application.Products.CreateProducts;
public record ProductCreatedEvent
{
    public Guid Id { get; set; }
    public decimal Price { get; init; }
}
