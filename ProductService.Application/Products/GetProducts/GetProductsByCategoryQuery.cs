using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.GetProducts;
/// <summary>
/// Represents a query to get products by category.
/// </summary>
/// <param name="Category">The category of the products to retrieve.</param>
/// <param name="Currency">The currency in which the product prices should be returned. Optional.</param>
/// <returns>A result containing an enumerable of <see cref="ProductResponse"/> objects or an <see cref="HttpClientCommunicationFailed"/> error.</returns>
public record GetProductsByCategoryQuery(
    string Category,
    string? Currency = null)
    : IRequest<Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>>;
