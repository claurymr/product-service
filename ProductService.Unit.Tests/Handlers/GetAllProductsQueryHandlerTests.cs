using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Services;
using ProductService.Application.Mappings;
using ProductService.Domain;
using ProductService.Infrastructure.Handlers.Products.GetProducts;
using Xunit;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Unit.Tests.Handlers;
public class GetAllProductsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IExchangeRateApiService> _exchangeRateApiServiceMock;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _exchangeRateApiServiceMock = _fixture.Freeze<Mock<IExchangeRateApiService>>();
        _handler = new GetAllProductsQueryHandler(_productRepositoryMock.Object, _exchangeRateApiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductsWithConvertedPrices_WhenCurrencyIsProvided()
    {
        // Arrange
        var products = _fixture
                        .CreateMany<Product>()
                        .ToList();
        var currency = "USD";
        var expectedExchangeRate = 60.14m;

        var query = new GetAllProductsQuery { Currency = currency };

        _productRepositoryMock
            .Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(products);
        _exchangeRateApiServiceMock
            .Setup(service => service.GetExchangeRateAsync(currency))
            .ReturnsAsync(expectedExchangeRate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(productResponse => productResponse, _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().HaveSameCount(products);
        resultValue.Should().AllSatisfy(response =>
        {
            var originalProduct = products.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price * expectedExchangeRate);
            response.Currency.Should().Be(currency);
        });
        _productRepositoryMock.Verify(repo => repo.GetProductsAsync(), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductsWithoutConversion_WhenCurrencyIsNotProvided()
    {
        // Arrange
        var products = _fixture
                        .CreateMany<Product>()
                        .ToList();
        var query = new GetAllProductsQuery();

        _productRepositoryMock
            .Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(productResponse => productResponse, _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().HaveSameCount(products);
        resultValue.Should().AllSatisfy(response =>
        {
            var originalProduct = products.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price);
            response.Currency.Should().BeNullOrEmpty();
        });

        _productRepositoryMock.Verify(repo => repo.GetProductsAsync(), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        // Arrange
        var query = new GetAllProductsQuery();
        _productRepositoryMock
            .Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(productResponse => productResponse, _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().BeEmpty();

        _productRepositoryMock.Verify(repo => repo.GetProductsAsync(), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOperationFailedResponse_WhenCurrencyApiFails()
    {
        // Arrange
        var currency = "USD";
        var query = new GetAllProductsQuery { Currency = currency };
        var products = _fixture.CreateMany<Product>().ToList();

        _productRepositoryMock
            .Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(products);
        _exchangeRateApiServiceMock
            .Setup(service => service.GetExchangeRateAsync(currency))
            .ReturnsAsync(new HttpClientCommunicationFailed(["Currency API failed"]));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultError = result.Match(
            _ => default!,
            error => error.MapToResponse());
        resultError.Should().NotBeNull();
        resultError.Should().BeOfType<OperationFailureResponse>();
        resultError.Errors.Should().ContainSingle(e => e.Message == "Currency API failed");

        _productRepositoryMock.Verify(repo => repo.GetProductsAsync(), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Once);
    }
}