namespace ProductService.Application.Contracts;


/// <summary>
/// Represents an error response from an exchange operation.
/// </summary>
public record ExchangeErrorResponse
{
    /// <summary>
    /// Gets the result of the exchange operation.
    /// </summary>
    public string Result { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of error that occurred during the exchange operation.
    /// </summary>
    public string ErrorType { get; init; } = string.Empty;
}