using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Application.Mappings;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;
using ProductService.Application.Services;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Handlers.PriceHistories;
public class GetPriceHistoryByProductIdQueryHandler(IPriceHistoryRepository priceHistoryRepository, IExchangeRateApiService exchangeRateApiService)
    : IRequestHandler<GetPriceHistoryByProductIdQuery, Result<IEnumerable<PriceHistoryResponse>, HttpClientCommunicationFailed>>
{
    public async Task<Result<IEnumerable<PriceHistoryResponse>, HttpClientCommunicationFailed>> Handle(GetPriceHistoryByProductIdQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        var priceHistories = await priceHistoryRepository.GetPriceHistoryByProductIdAsync(request.ProductId);
        if (request.Currency is not null)
        {
            var exchangeRateResult = await exchangeRateApiService.GetExchangeRateAsync(request.Currency);
            if (exchangeRateResult.IsError)
            {
                return exchangeRateResult.Match(_ => default!, error => error);
            }

            var exchangeRate = exchangeRateResult.Match(rate => rate, _ => 0m);
            return priceHistories.Select(product =>
                    {
                        return product.MapToResponse() with
                        {
                            OldPrice = product.OldPrice * exchangeRate,
                            NewPrice = product.NewPrice * exchangeRate,
                            Currency = request.Currency
                        };
                    }).ToList();
        }
        return priceHistories.MapToResponse().ToList();
    }
}