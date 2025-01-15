namespace ProductService.Application.Products.UpdateProducts;
public class ProductUpdatedEvent
{
    public Guid Id { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; init; }
}