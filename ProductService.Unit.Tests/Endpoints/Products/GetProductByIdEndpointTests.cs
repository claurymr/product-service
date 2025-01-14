using AutoFixture;
using AutoFixture.AutoMoq;
using FastEndpoints;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using ProductService.Api.Endpoints.Products;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Validation;
using Xunit;

namespace ProductService.Unit.Tests.Endpoints.Products;
public class GetProductByIdEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetProductByIdEndpoint _endpoint;

    public GetProductByIdEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new GetProductByIdEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productResponse = _fixture
                                .Build<ProductResponse>()
                                .With(p => p.Id, productId)
                                .With(p => p.Currency, default(string))
                                .Create();
        var request = _fixture
                        .Build<GetProductByIdQuery>()
                        .With(p => p.Id, productId)
                        .With(p => p.Currency, default(string))
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productResponse);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<ProductResponse>));
        (result.Result as Ok<ProductResponse>)!.Value.Should().Be(productResponse);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenProductExistsAndCurrencyIsProvided()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var currency = "USD";
        var expectedExchangeRate = 1.2m;
        var productResponse = _fixture
                                .Build<ProductResponse>()
                                .With(p => p.Id, productId)
                                .With(p => p.Currency, currency)
                                .With(p => p.Price, 100 * expectedExchangeRate)
                                .Create();
        var request = _fixture
                        .Build<GetProductByIdQuery>()
                        .With(p => p.Id, productId)
                        .With(p => p.Currency, currency)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productResponse);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<ProductResponse>));
        (result.Result as Ok<ProductResponse>)!.Value.Should().BeEquivalentTo(productResponse, options => options.Excluding(p => p.Price));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<GetProductByIdQuery>()
                        .With(p => p.Id, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecordNotFound([$"Product with Id {productId} not found."]));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NotFound<OperationFailureResponse>));
        (result.Result as NotFound<OperationFailureResponse>)!.Value!
            .Errors.Should().Contain(c => c.Message == $"Product with Id {productId} not found.");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnCustomResult_WhenExternalApiFails()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<GetProductByIdQuery>()
                        .With(p => p.Id, productId)
                        .Create();
        var expectedFailure = new HttpClientCommunicationFailed("Failed to communicate with external API.");
        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFailure);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(JsonHttpResult<OperationFailureResponse>));
        // Verify that the result contains the expected failure response
        (result.Result as JsonHttpResult<OperationFailureResponse>)!.Value!
            .Errors.Should().Contain(c => c.Message == "Failed to communicate with external API.");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}