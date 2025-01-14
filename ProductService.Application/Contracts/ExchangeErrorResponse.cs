namespace ProductService.Application.Contracts;
public record ExchangeErrorResponse
{
    public string Result { get; init; } = string.Empty;
    public string ErrorType { get; init; } = string.Empty;
}