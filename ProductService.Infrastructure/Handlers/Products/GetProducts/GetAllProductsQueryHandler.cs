using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Mappings;
using ProductService.Application.Services;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Handlers.Products.GetProducts;
/// <summary>
/// Handles the query to get all products.
/// </summary>
/// <param name="productRepository">The product repository to retrieve products from.</param>
/// <param name="exchangeRateApiService">The exchange rate API service to get exchange rates.</param>
/// <returns>
/// A handler for the <see cref="GetAllProductsQuery"/> that returns a result containing a collection 
/// of <see cref="ProductResponse"/> or an <see cref="HttpClientCommunicationFailed"/> error.
/// </returns>
public class GetAllProductsQueryHandler(IProductRepository productRepository, IExchangeRateApiService exchangeRateApiService)
    : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IExchangeRateApiService _exchangeRateApiService = exchangeRateApiService;

    public async Task<Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsAsync();
        if (request.Currency is not null)
        {
            var exchangeRateResult = await _exchangeRateApiService.GetExchangeRateAsync(request.Currency);
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