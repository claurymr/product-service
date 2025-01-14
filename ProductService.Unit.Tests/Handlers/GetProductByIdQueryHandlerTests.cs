using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Services;
using ProductService.Application.Validation;
using ProductService.Domain;
using ProductService.Infrastructure.Handlers.Products.GetProducts;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class GetProductByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IExchangeRateApiService> _exchangeRateApiServiceMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _exchangeRateApiServiceMock = _fixture.Freeze<Mock<IExchangeRateApiService>>();
        _handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object, _exchangeRateApiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductWithConvertedPrice_WhenCurrencyIsProvided()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        var currency = "USD";
        var expectedExchangeRate = 60.14m;
        var query = new GetProductByIdQuery(product.Id, currency);

        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product.Id))
            .ReturnsAsync(product);

        _exchangeRateApiServiceMock
            .Setup(service => service.GetExchangeRateAsync(currency))
            .ReturnsAsync(expectedExchangeRate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(
            productResponse => productResponse,
            _ => default!,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Id.Should().Be(product.Id);
        resultValue.Price.Should().Be(product.Price * expectedExchangeRate); 
        resultValue.Currency.Should().Be(currency);

        _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(product.Id), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(currency), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductWithoutConversion_WhenCurrencyIsNotProvided()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        var query = new GetProductByIdQuery(product.Id);

        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result
            .Match(
            productResponse => productResponse,
            _ => default!,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Id.Should().Be(product.Id);
        resultValue.Price.Should().Be(product.Price);
        resultValue.Currency.Should().BeNullOrEmpty();

        _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(product.Id), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnRecordNotFound_WhenProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetProductByIdQuery(productId);

        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(productId))
            .ReturnsAsync(default(Product)!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsWarning.Should().BeTrue();
        var resultNotFound = result.Match(
            _ => default!,
            _ => default!,
            notFound => notFound.MapToResponse());
        resultNotFound.Should().NotBeNull();
        resultNotFound.Should().BeOfType<OperationFailureResponse>();
        resultNotFound.Errors.Should().ContainSingle(e => e.Message == $"Product with Id {productId} not found.");

        _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(productId), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOperationFailedResponse_WhenCurrencyApiFails()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        var currency = "USD";
        var query = new GetProductByIdQuery(product.Id, currency);

        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product.Id))
            .ReturnsAsync(product);

        _exchangeRateApiServiceMock
            .Setup(service => service.GetExchangeRateAsync(currency))
            .ReturnsAsync(new HttpClientCommunicationFailed(["Currency API failed"]));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultError = result.Match(
            _ => default!,
            error => error.MapToResponse(),
            _ => default!);
        resultError.Should().NotBeNull();
        resultError.Should().BeOfType<OperationFailureResponse>();
        resultError.Errors.Should().ContainSingle(e => e.Message == "Currency API failed");

        _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(product.Id), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(currency), Times.Once);
    }
}