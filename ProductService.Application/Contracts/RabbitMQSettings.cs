namespace ProductService.Application.Contracts;
/// <summary>
/// Represents the settings required to connect to a RabbitMQ server.
/// </summary>
public record RabbitMQSettings
{
    /// <summary>
    /// Gets the hostname of the RabbitMQ server.
    /// </summary>
    public string HostName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the queue to connect to.
    /// </summary>
    public string QueueName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the username for authentication with the RabbitMQ server.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the password for authentication with the RabbitMQ server.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Gets the port number to connect to the RabbitMQ server.
    /// </summary>
    public string Port { get; init; } = string.Empty;
}