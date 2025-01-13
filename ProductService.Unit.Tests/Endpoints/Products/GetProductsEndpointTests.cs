using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using ProductService.Api.Endpoints.Products;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
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
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        var actualResponse = JsonSerializer.Deserialize<IEnumerable<ProductResponse>>(responseBody);
        actualResponse.Should().BeEquivalentTo(products);

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
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        var actualResponse = JsonSerializer.Deserialize<IEnumerable<ProductResponse>>(responseBody);
        actualResponse.Should().BeEquivalentTo(products, options => options.Excluding(p => p.Price));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        // Arrange
        var request = _fixture.Create<GetAllProductsQuery>();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        var actualResponse = JsonSerializer.Deserialize<IEnumerable<ProductResponse>>(responseBody);
        actualResponse.Should().BeEmpty();

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}