using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts;
using ProductService.Application.Services;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Services;
public class ExchangeRateApiService
    (HttpClient httpClient,
    ExchangeRateApiSettings settings,
    ILogger<ExchangeRateApiService> logger)
    : IExchangeRateApiService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ExchangeRateApiSettings _settings = settings;
    // add logger
    private readonly ILogger<ExchangeRateApiService> _logger = logger;
    public async Task<Result<decimal, HttpClientCommunicationFailed>> GetExchangeRateAsync(string targetCurrency)
    {
        _logger.LogInformation("Getting exchange rate {BaseAddress} for {targetCurrency} from {endpoint}",
            _httpClient.BaseAddress,
            targetCurrency,
            _settings.Endpoint);
        var response = await _httpClient.GetAsync($"{_settings.ApiKey}/{_settings.Endpoint}/{targetCurrency}");
        var content = await response.Content.ReadAsStringAsync();
        var exchangeRateResponse = JsonSerializer.Deserialize<ExchangeRateResponse>(content);

        if (exchangeRateResponse is null)
        {
            return new HttpClientCommunicationFailed(["Failed to communicate with the exchange rate API. Try again later."]);
        }

        if (exchangeRateResponse?.Result is "error")
        {
            var error = exchangeRateResponse?.ErrorType ?? "Could not obtain convertion rates";
            return new HttpClientCommunicationFailed(
                [$"{error}. Invalid currency code: {targetCurrency}"]);
        }

        if (exchangeRateResponse?.Result is "success" &&
            exchangeRateResponse.ConversionRates.TryGetValue(_settings.BaseConvertion, out var rate))
        {
            return rate;
        }

        return default(decimal);
    }
}