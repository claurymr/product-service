using AutoFixture;
using AutoFixture.AutoMoq;
using MassTransit;
using Moq;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Infrastructure.MessageBroker;
using Xunit;

namespace ProductService.Unit.Tests.MessageBroker;
public class EventBusTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly EventBus _eventBus;

    public EventBusTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _eventBus = new EventBus(_publishEndpointMock.Object);
    }

    [Fact]
    public async Task PublishAsync_WithProductCreatedEvent_ShouldPublishEvent()
    {
        // Arrange
        var @event = _fixture.Create<ProductCreatedEvent>();
        _publishEndpointMock
            .Setup(publishEndpoint => publishEndpoint.Publish(@event, new CancellationToken()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventBus.PublishAsync(@event);

        // Assert
        _publishEndpointMock
            .Verify(
                publishEndpoint => publishEndpoint.Publish(@event, It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithProductUpdatedEvent_ShouldPublishEvent()
    {
        // Arrange
        var @event = _fixture.Create<ProductUpdatedEvent>();
        _publishEndpointMock
            .Setup(publishEndpoint => publishEndpoint.Publish(@event, new CancellationToken()))
            .Returns(Task.CompletedTask);

        // Act
        await _eventBus.PublishAsync(@event);

        // Assert
        _publishEndpointMock
            .Verify(
                publishEndpoint => publishEndpoint.Publish(@event, It.IsAny<CancellationToken>()),
                Times.Once);
    }
}