using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class GetAllProductsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _handler = new GetAllProductsQueryHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductsWithConvertedPrices_WhenCurrencyIsProvided()
    {
        // Arrange
        var productResponses = _fixture
                                .CreateMany<ProductResponse>()
                                .ToList();
        var currency = "EUR";
        var expectedExchangeRate = 0m;

        var query = new GetAllProductsQuery { Currency = currency };

        _productRepositoryMock
            .Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(productResponses);
        // setup currency dependency for currency api

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveSameCount(productResponses);
        result.Should().AllSatisfy(response =>
        {
            var originalProduct = productResponses.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price * expectedExchangeRate);
            response.Currency.Should().Be(currency);
        });
        _productRepositoryMock.Verify(repo => repo.GetProductsAsync(), Times.Once);
        // verify currency api is called once
    }

    [Fact]
    public async Task Handle_ShouldReturnProductsWithoutConversion_WhenCurrencyIsNotProvided()
    {
        // Arrange
        var productResponses = _fixture
                                .CreateMany<ProductResponse>()
                                .ToList();
        var query = new GetAllProductsQuery();

        _productRepositoryMock
            .Setup(repo => repo.GetProductsAsync())
            .ReturnsAsync(productResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveSameCount(productResponses);
        result.Should().AllSatisfy(response =>
        {
            var originalProduct = productResponses.First(p => p.Id == response.Id);
            response.Price.Should().Be(originalProduct.Price);
            response.Currency.Should().BeNullOrEmpty();
        });

        _productRepositoryMock.Verify(repo => repo.GetProductsAsync(), Times.Once);
        // verify currency api is not called 
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
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _productRepositoryMock.Verify(repo => repo.GetProductsAsync(), Times.Once);
    }
}