using System.Text.Json.Serialization;

namespace ProductService.Application.Contracts;
public record ExchangeRateResponse
{
    [JsonPropertyName("result")]
    public string Result { get; init; } = string.Empty;

    [JsonPropertyName("base_code")]
    public string BaseCode { get; init; } = string.Empty;
    [JsonPropertyName("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; init; } = [];
    [JsonPropertyName("error-type")]
    public string ErrorType { get; set; } = string.Empty;
}