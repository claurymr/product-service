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
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        var actualResponse = JsonSerializer.Deserialize<IEnumerable<ProductResponse>>(responseBody);
        actualResponse.Should().BeEquivalentTo(products);

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
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseBody = await new StreamReader(_endpoint.HttpContext.Response.Body).ReadToEndAsync();
        var actualResponse = JsonSerializer.Deserialize<IEnumerable<ProductResponse>>(responseBody);
        actualResponse.Should().BeEquivalentTo(products, options => options.Excluding(p => p.Price));

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
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetProductsByCategoryQuery>(), It.IsAny<CancellationToken>()))
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