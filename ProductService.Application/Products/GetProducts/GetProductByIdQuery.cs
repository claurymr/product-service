using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.Products.GetProducts;
public record GetProductByIdQuery(
    Guid Id,
    string? Currency = null)
    : IRequest<ProductResponse>;