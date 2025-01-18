using MassTransit;
using ProductService.Application.EventBus;

namespace ProductService.Infrastructure.MessageBroker;
/// <summary>
/// Represents an event bus that is responsible for publishing events.
/// </summary>
/// <param name="publishEndpoint">The endpoint used to publish events.</param>
public sealed class EventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    /// Publishes an event asynchronously to the message broker.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to be published.</typeparam>
    /// <param name="event">The event to be published.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class =>
        _publishEndpoint.Publish(@event, cancellationToken);

}