namespace ProductService.Application.Contracts;
/// <summary>
/// Represents a response containing validation failures.
/// </summary>
public record ValidationFailureResponse
{
    public required IEnumerable<ValidationResponse> Errors { get; init; }
}

public record ValidationResponse
{
    public required string PropertyName { get; init; }

    public required string Message { get; init; }
}