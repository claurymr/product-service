using ProductService.Domain.Enums;

namespace ProductService.Domain;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public CategoryType Category { get; set; }
    public string Sku { get; set; } = string.Empty;
}