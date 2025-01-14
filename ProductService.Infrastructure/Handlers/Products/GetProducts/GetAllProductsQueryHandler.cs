using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Mappings;
using ProductService.Application.Services;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Handlers.Products.GetProducts;
public class GetAllProductsQueryHandler(IProductRepository productRepository, IExchangeRateApiService exchangeRateApiService)
    : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>>
{
    public async Task<Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        var products = await productRepository.GetProductsAsync();
        if (request.Currency is not null)
        {
            var exchangeRateResult = await exchangeRateApiService.GetExchangeRateAsync(request.Currency);
            if (exchangeRateResult.IsError)
            {
                return exchangeRateResult.Match(_ => default!, error => error);
            }

            var exchangeRate = exchangeRateResult.Match(rate => rate, _ => 0m);
            return products.Select(product =>
                    {
                        return product.MapToResponse() with
                        {
                            Price = product.Price * exchangeRate,
                            Currency = request.Currency
                        };
                    }).ToList();
        }

        return products.MapToResponse().ToList();
    }
}