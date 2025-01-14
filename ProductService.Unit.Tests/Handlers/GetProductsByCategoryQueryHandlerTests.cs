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
public class GetProductsByCategoryQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IExchangeRateApiService> _exchangeRateApiServiceMock;
    private readonly GetProductsByCategoryQueryHandler _handler;

    public GetProductsByCategoryQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _exchangeRateApiServiceMock = _fixture.Freeze<Mock<IExchangeRateApiService>>();
        _handler = new GetProductsByCategoryQueryHandler(_productRepositoryMock.Object, _exchangeRateApiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductsWithConvertedPrices_WhenCurrencyIsProvided()
    {
        // Arrange
        var category = "Electronics";
        var currency = "USD";
        var expectedExchangeRate = 60.14m;

        var products = _fixture
                        .Build<Product>()
                        .With(p => p.Category, category)
                        .CreateMany(5)
                        .ToList();

        var query = new GetProductsByCategoryQuery(category, currency);

        _productRepositoryMock
            .Setup(repo => repo.GetProductsByCategoryAsync(category))
            .ReturnsAsync(products);

        _exchangeRateApiServiceMock
            .Setup(service => service.GetExchangeRateAsync(currency))
            .ReturnsAsync(expectedExchangeRate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(
            productResponse => productResponse,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().HaveSameCount(products);
        resultValue.Should().AllSatisfy(response =>
        {
            var originalProduct = products.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price * expectedExchangeRate);
            response.Currency.Should().Be(currency);
        });

        _productRepositoryMock.Verify(repo => repo.GetProductsByCategoryAsync(category), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(currency), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductsWithoutConversion_WhenCurrencyIsNotProvided()
    {
        // Arrange
        var category = "Books";

        var products = _fixture
                        .Build<Product>()
                        .With(p => p.Category, category)
                        .CreateMany(3)
                        .ToList();

        var query = new GetProductsByCategoryQuery(category);

        _productRepositoryMock
            .Setup(repo => repo.GetProductsByCategoryAsync(category))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(
            productResponse => productResponse,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().HaveSameCount(products);
        resultValue.Should().AllSatisfy(response =>
        {
            var originalProduct = products.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price);
            response.Currency.Should().BeNullOrEmpty();
        });

        _productRepositoryMock.Verify(repo => repo.GetProductsByCategoryAsync(category), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFoundForCategory()
    {
        // Arrange
        var category = "NonExistentCategory";
        var query = new GetProductsByCategoryQuery(category);

        _productRepositoryMock
            .Setup(repo => repo.GetProductsByCategoryAsync(category))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(
            productResponse => productResponse,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().BeEmpty();

        _productRepositoryMock.Verify(repo => repo.GetProductsByCategoryAsync(category), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOperationFailedResponse_WhenCurrencyApiFails()
    {
        // Arrange
        var category = "Electronics";
        var currency = "USD";
        var products = _fixture
                        .Build<Product>()
                        .With(p => p.Category, category)
                        .CreateMany(5)
                        .ToList();

        var query = new GetProductsByCategoryQuery(category, currency);

        _productRepositoryMock
            .Setup(repo => repo.GetProductsByCategoryAsync(category))
            .ReturnsAsync(products);

        _exchangeRateApiServiceMock
            .Setup(service => service.GetExchangeRateAsync(currency))
            .ReturnsAsync(new HttpClientCommunicationFailed("Failed to get exchange rate."));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultError = result.Match(
            _ => default!,
            error => error.MapToResponse());
        resultError.Should().NotBeNull();
        resultError.Should().BeOfType<OperationFailureResponse>();
        resultError.Errors.Should().ContainSingle(e => e.Message == "Failed to get exchange rate.");

        _productRepositoryMock.Verify(repo => repo.GetProductsByCategoryAsync(category), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(currency), Times.Once);
    }
}