using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.ProductPriceHistories.GetPriceHistories;
public record GetPriceHistoryByProductIdQuery(
    Guid ProductId,
    string? Currency = null)
    : IRequest<Result<IEnumerable<PriceHistoryResponse>, HttpClientCommunicationFailed>>;