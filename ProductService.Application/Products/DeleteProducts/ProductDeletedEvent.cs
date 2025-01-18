namespace ProductService.Application.Products.DeleteProducts;
public class ProductDeletedEvent
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
}