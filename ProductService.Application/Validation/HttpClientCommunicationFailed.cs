namespace ProductService.Application.Validation;
public record HttpClientCommunicationFailed(string[] Messages)
{
    public HttpClientCommunicationFailed(string message) : this([message])
    {
    }
}