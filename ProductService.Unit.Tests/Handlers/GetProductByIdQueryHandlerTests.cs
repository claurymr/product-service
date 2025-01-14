using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Mappings;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Domain;
using ProductService.Infrastructure.Handlers.Products.GetProducts;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class GetProductByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    // add currency httpclient mock field
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        // initialize currency httpclient mock
        _handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductWithConvertedPrice_WhenCurrencyIsProvided()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        var currency = "EUR";
        var expectedExchangeRate = 0m;

        var query = new GetProductByIdQuery(product.Id, currency);

        _productRepositoryMock
            .Setup(repo => repo.GetProductByIdAsync(product.Id))
            .ReturnsAsync(product);

        // setup currency api call

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(productResponse => productResponse, _ => default!);
        result.Should().NotBeNull();
        resultValue.Id.Should().Be(product.Id);
        resultValue.Price.Should().Be(product.Price * expectedExchangeRate); 
        resultValue.Currency.Should().Be(currency);

        _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(product.Id), Times.Once);
        // verify currency api is called once
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
        var resultValue = result.Match(productResponse => productResponse, _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Id.Should().Be(product.Id);
        resultValue.Price.Should().Be(product.Price);
        resultValue.Currency.Should().BeNullOrEmpty();

        _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(product.Id), Times.Once);
        // verify currency api is not called 
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
        result.IsError.Should().BeTrue();
        var resultNotFound = result.Match(
            _ => default!, 
            notFound => notFound.MapToResponse());
        resultNotFound.Should().NotBeNull();
        resultNotFound.Should().BeOfType<RecordNotFound>();
        resultNotFound.Errors.Should().ContainSingle(e => e.Message == $"Product with ID {productId} not found.");
        _productRepositoryMock.Verify(repo => repo.GetProductByIdAsync(productId), Times.Once);
    }
}