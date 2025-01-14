namespace ProductService.Application.Contracts;
public record ExchangeRateApiSettings
{
    public string BaseUrl { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
    public string BaseConvertion { get; init; } = string.Empty;
}