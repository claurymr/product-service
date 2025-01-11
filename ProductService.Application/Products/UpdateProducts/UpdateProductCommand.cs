using MediatR;

namespace ProductService.Application.Products.UpdateProducts;
public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    string Sku) : IRequest<Guid>;