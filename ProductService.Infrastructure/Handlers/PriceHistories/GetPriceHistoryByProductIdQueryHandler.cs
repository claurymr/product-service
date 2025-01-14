using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Application.Mappings;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;

namespace ProductService.Infrastructure.Handlers.PriceHistories;
public class GetPriceHistoryByProductIdQueryHandler(IPriceHistoryRepository priceHistoryRepository)
    : IRequestHandler<GetPriceHistoryByProductIdQuery, IEnumerable<PriceHistoryResponse>>
{
    // Declare httpclient private field
    public async Task<IEnumerable<PriceHistoryResponse>> Handle(GetPriceHistoryByProductIdQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        var priceHistories = await priceHistoryRepository.GetPriceHistoryByProductIdAsync(request.ProductId);
        // get products with applied calculation of price, and mapped to currency
        return priceHistories.MapToResponse();
    }
}