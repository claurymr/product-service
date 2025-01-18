using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Services;
/// <summary>
/// Defines a service for retrieving exchange rates from an external API.
/// </summary>
public interface IExchangeRateApiService
{
    /// <summary>
    /// Asynchronously retrieves the exchange rate for the specified target currency.
    /// </summary>
    /// <param name="targetCurrency">The currency code for which the exchange rate is to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T, TError}"/> object 
    /// with the exchange rate as a decimal if successful, or an <see cref="HttpClientCommunicationFailed"/> error if the operation fails.</returns>
    Task<Result<decimal, HttpClientCommunicationFailed>> GetExchangeRateAsync(string targetCurrency);
}