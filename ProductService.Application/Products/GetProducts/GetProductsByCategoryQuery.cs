using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.Products.GetProducts;
public record GetProductsByCategoryQuery(
    string Category,
    string? Currency = null)
    : IRequest<IEnumerable<ProductResponse>>;