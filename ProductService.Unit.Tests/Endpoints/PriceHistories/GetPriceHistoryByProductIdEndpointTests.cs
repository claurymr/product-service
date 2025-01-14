using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using FastEndpoints;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProductService.Api.Endpoints.PriceHistories;
using ProductService.Application.Contracts;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;
using ProductService.Application.Validation;
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
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<PriceHistoryResponse>>));
        (result.Result as Ok<IEnumerable<PriceHistoryResponse>>)!.Value
            .Should()
            .BeEquivalentTo(priceHistories, options => options.Excluding(p => p.OldPrice).Excluding(p => p.NewPrice));

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
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<PriceHistoryResponse>>));
        (result.Result as Ok<IEnumerable<PriceHistoryResponse>>)!.Value
            .Should()
            .BeEquivalentTo(priceHistories, options => options.Excluding(p => p.OldPrice).Excluding(p => p.NewPrice));

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
            .ReturnsAsync(new List<PriceHistoryResponse>());

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<PriceHistoryResponse>>));
        (result.Result as Ok<IEnumerable<PriceHistoryResponse>>)!.Value
            .Should()
            .BeEmpty();

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPriceHistoryByProductId_ShouldReturnCustomResult_WhenExternalApiFails()
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
            .ReturnsAsync(new HttpClientCommunicationFailed("Failed to communicate with external API."));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(JsonHttpResult<OperationFailureResponse>));
        (result.Result as JsonHttpResult<OperationFailureResponse>)!.StatusCode
            .Should()
            .Be(StatusCodes.Status500InternalServerError);

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}