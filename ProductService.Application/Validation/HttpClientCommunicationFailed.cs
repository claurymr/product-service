namespace ProductService.Application.Validation;

/// <summary>
/// Represents a failure in HTTP client communication.
/// </summary>
/// <param name="Messages">An array of error messages describing the failure.</param>
public record HttpClientCommunicationFailed(string[] Messages)
{
    public HttpClientCommunicationFailed(string message) : this([message])
    {
    }
}