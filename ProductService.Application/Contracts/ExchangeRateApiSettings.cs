namespace ProductService.Application.Contracts;

/// <summary>
/// Represents the settings required to interact with the Exchange Rate API.
/// </summary>
public record ExchangeRateApiSettings
{
    /// <summary>
    /// Gets the base URL of the Exchange Rate API.
    /// </summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the API key used for authenticating requests to the Exchange Rate API.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets the endpoint of the Exchange Rate API.
    /// </summary>
    public string Endpoint { get; init; } = string.Empty;

    /// <summary>
    /// Gets the base conversion currency for the Exchange Rate API.
    /// </summary>
    public string BaseConvertion { get; init; } = string.Empty;
}