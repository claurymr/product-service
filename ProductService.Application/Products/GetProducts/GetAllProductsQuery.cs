using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.GetProducts;
public record GetAllProductsQuery(string? Currency = null) 
    : IRequest<Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>>;
