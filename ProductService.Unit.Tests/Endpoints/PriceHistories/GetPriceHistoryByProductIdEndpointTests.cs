using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using ProductService.Api.Endpoints.PriceHistories;
using ProductService.Application.Contracts;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;
using Xunit;

namespace ProductService.Unit.Tests.Endpoints.PriceHistories;
public class GetPriceHistoryByProductIdEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetPriceHistoryByProductIdEndpoint _endpoint;

    public GetPriceHistoryByProductIdEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new GetPriceHistoryByProductIdEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetPriceHistoryByProductId_ShouldReturnPriceHistories_WhenPriceHistoriesExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var priceHistories = _fixture
                                .Build<PriceHistoryResponse>()
                                .With(p => p.ProductId, productId)
                                .With(p => p.Currency, default(string))
                                .CreateMany(3)
                                .ToList();
        var request = _fixture
                        .Build<GetPriceHistoryByProductIdQuery>()
                        .With(p => p.ProductId, productId)
                        .With(p => p.Currency, default(string))
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetPriceHistoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(priceHistories);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        var actualResponse = JsonSerializer.Deserialize<IEnumerable<PriceHistoryResponse>>(responseBody);
        actualResponse.Should().BeEquivalentTo(priceHistories);

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPriceHistoryByProductId_ShouldReturnPriceHistories_WhenPriceHistoriesExistAndCurrencyIsProvided()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var currency = "USD";
        var expectedExchangeRate = 1.2m;
        var priceHistories = _fixture
                                .Build<PriceHistoryResponse>()
                                .With(p => p.ProductId, productId)
                                .With(p => p.Currency, currency)
                                .With(p => p.OldPrice, _fixture.Create<decimal>() * expectedExchangeRate)
                                .With(p => p.NewPrice, _fixture.Create<decimal>() * expectedExchangeRate)
                                .CreateMany(3)
                                .ToList();
        var request = _fixture
                        .Build<GetPriceHistoryByProductIdQuery>()
                        .With(p => p.ProductId, productId)
                        .With(p => p.Currency, currency)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetPriceHistoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(priceHistories);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        var actualResponse = JsonSerializer.Deserialize<IEnumerable<PriceHistoryResponse>>(responseBody);
        actualResponse.Should().BeEquivalentTo(priceHistories, options => options.Excluding(p => p.OldPrice).Excluding(p => p.NewPrice));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPriceHistoryByProductId_ShouldReturnEmptyList_WhenPriceHistoriesDoNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<GetPriceHistoryByProductIdQuery>()
                        .With(p => p.ProductId, productId)
                        .With(p => p.Currency, default(string))
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetPriceHistoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        responseBody.Should().BeEmpty();

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}