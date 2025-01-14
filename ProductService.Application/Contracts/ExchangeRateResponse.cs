namespace ProductService.Application.Contracts;
public record ExchangeRateResponse
{
    public string Result { get; init; } = string.Empty;
    public string BaseCode { get; init; } = string.Empty;
    public Dictionary<string, decimal> ConversionRates { get; init; } = [];
}