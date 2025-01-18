using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.GetProducts;
/// <summary>
/// Represents a query to get a product by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Currency">The currency in which the product price should be returned. Optional.</param>
/// <returns>A result containing the product response, or an error indicating that the HTTP client communication failed or the record was not found.</returns>
public record GetProductByIdQuery(
    Guid Id,
    string? Currency = null)
    : IRequest<ResultWithWarning<ProductResponse, HttpClientCommunicationFailed, RecordNotFound>>;