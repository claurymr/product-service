using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Services;
public interface IExchangeRateApiService
{
    Task<Result<decimal, HttpClientCommunicationFailed>> GetExchangeRateAsync(string targetCurrency);
}