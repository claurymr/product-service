using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.ProductPriceHistories.GetPriceHistories;
public record GetPriceHistoryByProductIdQuery(
    Guid ProductId,
    string? Currency = null)
    : IRequest<IEnumerable<PriceHistoryResponse>>;