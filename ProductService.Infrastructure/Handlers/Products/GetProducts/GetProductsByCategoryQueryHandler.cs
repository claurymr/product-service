using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Mappings;
using ProductService.Application.Validation;
using ProductService.Application.Services;

namespace ProductService.Infrastructure.Handlers.Products.GetProducts;
public class GetProductsByCategoryQueryHandler(IProductRepository productRepository, IExchangeRateApiService exchangeRateApiService)
    : IRequestHandler<GetProductsByCategoryQuery, Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IExchangeRateApiService _exchangeRateApiService = exchangeRateApiService;
    public async Task<Result<IEnumerable<ProductResponse>, HttpClientCommunicationFailed>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsByCategoryAsync(request.Category);
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