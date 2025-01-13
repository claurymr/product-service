namespace ProductService.Application.Contracts;
public record OperationFailureResponse
{
    public required IEnumerable<OperationResponse> Errors { get; init; }
}

public record OperationResponse
{
    public required string Message { get; init; }
}