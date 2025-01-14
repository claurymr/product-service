using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using FastEndpoints;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProductService.Api.Endpoints.Products;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Validation;
using Xunit;

namespace ProductService.Unit.Tests.Endpoints.Products;
public class GetProductsEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetProductsEndpoint _endpoint;

    public GetProductsEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new GetProductsEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProducts_WhenProductsExist()
    {
        // Arrange
        var products = _fixture
                        .Build<ProductResponse>()
                        .With(p => p.Currency, default(string))
                        .CreateMany()
                        .ToList();
        var request = _fixture
                        .Build<GetAllProductsQuery>()
                        .With(q => q.Currency, default(string))
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<ProductResponse>>));
        (result.Result as Ok<IEnumerable<ProductResponse>>)!.Value
            .Should()
            .BeEquivalentTo(products, options => options.Excluding(p => p.Price));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProducts_WhenProductsExistsAndCurrencyIsProvided()
    {
        // Arrange
        var currency = "USD";
        var expectedExchangeRate = 1.2m;
        var products = _fixture
                        .Build<ProductResponse>()
                        .With(p => p.Currency, currency)
                        .With(p => p.Price, _fixture.Create<decimal>() * expectedExchangeRate)
                        .CreateMany()
                        .ToList();
        var request = _fixture
                        .Build<GetAllProductsQuery>()
                        .With(q => q.Currency, currency)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<ProductResponse>>));
        (result.Result as Ok<IEnumerable<ProductResponse>>)!.Value
            .Should()
            .BeEquivalentTo(products, options => options.Excluding(p => p.Price));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        // Arrange
        var request = _fixture.Create<GetAllProductsQuery>();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse>());

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<ProductResponse>>));
        (result.Result as Ok<IEnumerable<ProductResponse>>)!.Value
            .Should()
            .BeEmpty();

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProblemDetails_WhenExternalApiFails()
    {
        // Arrange
        var request = _fixture.Create<GetAllProductsQuery>();
        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpClientCommunicationFailed("Failed to communicate with external API."));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(JsonHttpResult<OperationFailureResponse>));
        (result.Result as JsonHttpResult<OperationFailureResponse>)!.Value!.Errors
            .Should().Contain(c => c.Message == "Failed to communicate with external API.");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}