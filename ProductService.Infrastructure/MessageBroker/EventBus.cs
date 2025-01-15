using MassTransit;
using ProductService.Application.EventBus;

namespace ProductService.Infrastructure.MessageBroker;
public sealed class EventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class =>
        _publishEndpoint.Publish(@event, cancellationToken);

}