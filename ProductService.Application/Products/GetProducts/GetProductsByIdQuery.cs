using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.Products.GetProducts;
public record GetProductsByIdQuery(
    Guid Id,
    string? Currency = null)
    : IRequest<IEnumerable<ProductResponse>>;