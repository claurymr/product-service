using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.GetProducts;
public record GetProductByIdQuery(
    Guid Id,
    string? Currency = null)
    : IRequest<ResultWithWarning<ProductResponse, HttpClientCommunicationFailed, RecordNotFound>>;