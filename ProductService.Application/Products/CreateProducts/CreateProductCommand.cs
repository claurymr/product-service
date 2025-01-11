using MediatR;

namespace ProductService.Application.Products.CreateProducts;
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Category,
    string Sku)
    : IRequest<Guid>;
