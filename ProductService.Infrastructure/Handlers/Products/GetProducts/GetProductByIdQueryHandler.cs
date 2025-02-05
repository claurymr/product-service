using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Application.Mappings;
using ProductService.Application.Services;

namespace ProductService.Infrastructure.Handlers.Products.GetProducts;

/// <summary>
/// Handles the query to get a product by its ID.
/// </summary>
/// <param name="productRepository">The repository to access product data.</param>
/// <param name="exchangeRateApiService">The service to get exchange rate information.</param>
/// <returns>
/// A handler for the GetProductByIdQuery that returns a ResultWithWarning containing the ProductResponse, 
/// HttpClientCommunicationFailed, or RecordNotFound.
/// </returns>
public class GetProductByIdQueryHandler(IProductRepository productRepository, IExchangeRateApiService exchangeRateApiService)
    : IRequestHandler<GetProductByIdQuery, ResultWithWarning<ProductResponse, HttpClientCommunicationFailed, RecordNotFound>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IExchangeRateApiService _exchangeRateApiService = exchangeRateApiService;

    public async Task<ResultWithWarning<ProductResponse, HttpClientCommunicationFailed, RecordNotFound>> 
        Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(request.Id);
        if(product is null)
        {
            return new RecordNotFound([$"Product with Id {request.Id} not found."]);
        }
        if (request.Currency is not null)
        {
            var exchangeRateResult = await _exchangeRateApiService.GetExchangeRateAsync(request.Currency);
            if (exchangeRateResult.IsError)
            {
                return exchangeRateResult.Match(_ => default!, failed => failed);
            }

            var exchangeRate = exchangeRateResult.Match(rate => rate, _ => 0m);
            return product.MapToResponse() with
                {
                    Price = product.Price * exchangeRate,
                    Currency = request.Currency
                };
        }
        return product.MapToResponse();
    }
}