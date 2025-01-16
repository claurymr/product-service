namespace ProductService.Application.Products.UpdateProducts;
public class ProductUpdatedEvent
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
}