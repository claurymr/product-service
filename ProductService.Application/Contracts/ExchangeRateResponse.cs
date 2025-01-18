using System.Text.Json.Serialization;

namespace ProductService.Application.Contracts;
/// <summary>
/// Represents the response for an exchange rate request.
/// </summary>
public record ExchangeRateResponse
{
    /// <summary>
    /// Gets the result of the exchange rate request.
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; init; } = string.Empty;

    /// <summary>
    /// Gets the base currency code for the exchange rate.
    /// </summary>
    [JsonPropertyName("base_code")]
    public string BaseCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the conversion rates for different currencies.
    /// </summary>
    [JsonPropertyName("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; init; } = new();

    /// <summary>
    /// Gets the error type if the exchange rate request failed.
    /// </summary>
    [JsonPropertyName("error-type")]
    public string ErrorType { get; set; } = string.Empty;
}