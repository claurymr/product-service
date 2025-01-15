namespace ProductService.Application.Contracts;
public record RabbitMQSettings
{
    public string HostName { get; init; } = string.Empty;
    public string QueueName { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Port { get; init; } = string.Empty;
}