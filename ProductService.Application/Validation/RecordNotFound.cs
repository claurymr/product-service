namespace ProductService.Application.Validation;
/// <summary>
/// Represents an error indicating that a record was not found.
/// </summary>
/// <param name="Messages">An array of messages describing the error.</param>
public record RecordNotFound(string[] Messages)
{
    public RecordNotFound(string message) : this([message])
    {
    }
}