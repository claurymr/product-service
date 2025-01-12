using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.CreateProducts;
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Category,
    string Sku)
    : IRequest<Result<Guid, ValidationFailed>>;
