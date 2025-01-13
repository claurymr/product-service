using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Domain;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class GetProductsByCategoryQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    // add currency httpclient mock field
    private readonly GetProductsByCategoryQueryHandler _handler;

    public GetProductsByCategoryQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        // initialize currency httpclient mock
        _handler = new GetProductsByCategoryQueryHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductsWithConvertedPrices_WhenCurrencyIsProvided()
    {
        // Arrange
        var category = "Electronics";
        var currency = "USD";
        var expectedExchangeRate = 0m;

        var products = _fixture
                        .Build<Product>()
                        .With(p => p.Category, category)
                        .CreateMany(5)
                        .ToList();

        var query = new GetProductsByCategoryQuery(category, currency);

        _productRepositoryMock
            .Setup(repo => repo.GetProductsByCategoryAsync(category))
            .ReturnsAsync(products);

        // setup currency api call

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveSameCount(products);
        result.Should().AllSatisfy(response =>
        {
            var originalProduct = products.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price * expectedExchangeRate);
            response.Currency.Should().Be(currency);
        });

        _productRepositoryMock.Verify(repo => repo.GetProductsByCategoryAsync(category), Times.Once);
        // verify currency api is called once
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
        result.Should().NotBeNull();
        result.Should().HaveSameCount(products);
        result.Should().AllSatisfy(response =>
        {
            var originalProduct = products.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price);
            response.Currency.Should().BeNullOrEmpty();
        });

        _productRepositoryMock.Verify(repo => repo.GetProductsByCategoryAsync(category), Times.Once);
        // verify currency api is not called
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
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _productRepositoryMock.Verify(repo => repo.GetProductsByCategoryAsync(category), Times.Once);
    }
}