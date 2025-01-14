using System.Text.Json;
using Microsoft.Extensions.Options;
using ProductService.Application.Contracts;
using ProductService.Application.Services;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Services;
public class ExchangeRateApiService(HttpClient httpClient, IOptions<ExchangeRateApiSettings> exchangeRateApiOptions) : IExchangeRateApiService
{
    public async Task<Result<decimal, HttpClientCommunicationFailed>> GetExchangeRateAsync(string targetCurrency)
    {
        var settings = exchangeRateApiOptions.Value;
        var response = await httpClient.GetAsync($"{settings.Endpoint}/{targetCurrency}");
        if (!response.IsSuccessStatusCode)
        {
            return new HttpClientCommunicationFailed(["Failed to communicate with the exchange rate API. Try again later."]);
        }
        var content = await response.Content.ReadAsStringAsync();
        var exchangeRateResponse = JsonSerializer.Deserialize<ExchangeRateResponse>(content);
        if (exchangeRateResponse?.Result is "success" &&
            exchangeRateResponse.ConversionRates.TryGetValue(settings.BaseConvertion, out var rate))
        {
            return rate;
        }
        return default(decimal);
    }
}