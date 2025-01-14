using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.UpdateProducts;
public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    string Sku)
    : IRequest<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>;
