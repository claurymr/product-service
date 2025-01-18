using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.GetProducts;
/// <summary>
/// Represents a query to retrieve all products.
/// </summary>
/// <param name="Currency">the currency parameter to filter the products by currency. Options.</param>
/// <returns>A result containing an enumerable of <see cref="ProductResponse"/> or an <see cref="HttpClientCommunicationFailed"/> error.</returns>
public record GetAllProductsQuery(string? Currency = null) 
    : IRequest<Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>>;
