using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.GetProducts;
public record GetProductsByCategoryQuery(
    string Category,
    string? Currency = null)
    : IRequest<Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>>;