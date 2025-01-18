namespace ProductService.Application.Contracts;
/// <summary>
/// Represents a response indicating the failure of an operation.
/// </summary>
public record OperationFailureResponse
{
    public required IEnumerable<OperationResponse> Errors { get; init; }
}

public record OperationResponse
{
    public required string Message { get; init; }
}