using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.ProductPriceHistories.GetPriceHistories;
/// <summary>
/// Query to get the price history of a product by its ID.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Currency">The currency in which the price history should be retrieved. Optional parameter.</param>
/// <returns>A result containing an enumerable of <see cref="PriceHistoryResponse"/> or an <see cref="HttpClientCommunicationFailed"/> error.</returns>
public record GetPriceHistoryByProductIdQuery(
    Guid ProductId,
    string? Currency = null)
    : IRequest<Result<IEnumerable<PriceHistoryResponse>, HttpClientCommunicationFailed>>;
