using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Repositories;

namespace ProductService.Application.ProductPriceHistories.GetPriceHistories;
public class GetPriceHistoryByProductIdQueryHandler(IPriceHistoryRepository priceHistoryRepository)
    : IRequestHandler<GetPriceHistoryByProductIdQuery, IEnumerable<PriceHistoryResponse>>
{
    // Declare repository private field
    // Declare httpclient private field
    // Call main ctor and initialize repository and httpclient
    public Task<IEnumerable<PriceHistoryResponse>> Handle(GetPriceHistoryByProductIdQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        // get products with applied calculation of price, and mapped to currency
        // return list of products
        throw new NotImplementedException();
    }
}