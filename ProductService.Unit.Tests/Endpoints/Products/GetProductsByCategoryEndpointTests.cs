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
public class GetProductsByCategoryEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetProductsByCategoryEndpoint _endpoint;

    public GetProductsByCategoryEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new GetProductsByCategoryEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnProducts_WhenProductsExist()
    {
        // Arrange
        var category = "Electronics";
        var products = _fixture
                        .Build<ProductResponse>()
                        .With(p => p.Category, category)
                        .With(p => p.Currency, default(string))
                        .CreateMany(5)
                        .ToList();
        var request = _fixture
                        .Build<GetProductsByCategoryQuery>()
                        .With(q => q.Category, category)
                        .With(q => q.Currency, default(string))
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetProductsByCategoryQuery>(), It.IsAny<CancellationToken>()))
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
    public async Task GetProductsByCategory_ShouldReturnProducts_WhenProductsExistsAndCurrencyIsProvided()
    {
        // Arrange
        var category = "Electronics";
        var currency = "USD";
        var expectedExchangeRate = 1.2m;
        var products = _fixture
                        .Build<ProductResponse>()
                        .With(p => p.Category, category)
                        .With(p => p.Currency, currency)
                        .With(p => p.Price, _fixture.Create<decimal>() * expectedExchangeRate)
                        .CreateMany(5)
                        .ToList();
        var request = _fixture
                        .Build<GetProductsByCategoryQuery>()
                        .With(q => q.Category, category)
                        .With(q => q.Currency, currency)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetProductsByCategoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<ProductResponse>>));
        (result.Result as Ok<IEnumerable<ProductResponse>>)!.Value.Should().BeEquivalentTo(products, options => options.Excluding(p => p.Price));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnEmptyList_WhenProductsDoNotExist()
    {
        // Arrange
        var category = "Electronics";
        var request = _fixture
                        .Build<GetProductsByCategoryQuery>()
                        .With(q => q.Category, category)
                        .Without(p => p.Currency)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse>());

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<ProductResponse>>));
        (result.Result as Ok<IEnumerable<ProductResponse>>)!.Value.Should().BeEmpty();

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnProblemDetails_WhenExternalApiFails()
    {
        // Arrange
        var category = "Electronics";
        var request = _fixture
                        .Build<GetProductsByCategoryQuery>()
                        .With(q => q.Category, category)
                        .Without(p => p.Currency)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpClientCommunicationFailed("Failed to communicate with external api."));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(JsonHttpResult<OperationFailureResponse>));
        (result.Result as JsonHttpResult<OperationFailureResponse>)!.Value!.Errors
            .Should().Contain(c => c.Message == "Failed to communicate with external api.");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}