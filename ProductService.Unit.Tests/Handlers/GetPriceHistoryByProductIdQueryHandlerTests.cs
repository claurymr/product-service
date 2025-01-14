using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;
using ProductService.Application.Repositories;
using ProductService.Application.Services;
using ProductService.Domain;
using ProductService.Infrastructure.Handlers.PriceHistories;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class GetPriceHistoryByProductIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IPriceHistoryRepository> _priceHistoryRepositoryMock;
    private readonly Mock<IExchangeRateApiService> _exchangeRateApiServiceMock;
    private readonly GetPriceHistoryByProductIdQueryHandler _handler;

    public GetPriceHistoryByProductIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _priceHistoryRepositoryMock = _fixture.Freeze<Mock<IPriceHistoryRepository>>();
        _exchangeRateApiServiceMock = _fixture.Freeze<Mock<IExchangeRateApiService>>();
        _handler = new GetPriceHistoryByProductIdQueryHandler(_priceHistoryRepositoryMock.Object, _exchangeRateApiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPriceHistoriesWithCurrency_WhenProductIdAndCurrencyIsProvided()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var currency = "USD";
        var expectedExchangeRate = 60.14m;
        var priceHistories = _fixture
                        .Build<PriceHistory>()
                        .With(p => p.ProductId, productId)
                        .With(ph => ph.Product,
                            _fixture.Build<Product>()
                            .With(p => p.Id, productId)
                            .Create())
                        .CreateMany(5)
                        .ToList();

        var query = new GetPriceHistoryByProductIdQuery(productId, currency);

        _priceHistoryRepositoryMock
            .Setup(repo => repo.GetPriceHistoryByProductIdAsync(productId))
            .ReturnsAsync(priceHistories);

        _exchangeRateApiServiceMock
            .Setup(service => service.GetExchangeRateAsync(currency))
            .ReturnsAsync(expectedExchangeRate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(
            priceHistoryResponse => priceHistoryResponse,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().HaveSameCount(priceHistories);
        resultValue.Should().AllSatisfy(response =>
        {
            var originalPriceHistory = priceHistories.First(p => p.Id == response.Id);
            response.ProductId.Should().Be(productId);
            response.Currency.Should().Be(currency);
            response.OldPrice.Should().Be(originalPriceHistory.OldPrice * expectedExchangeRate);
            response.NewPrice.Should().Be(originalPriceHistory.NewPrice * expectedExchangeRate);
        });

        _priceHistoryRepositoryMock.Verify(repo => repo.GetPriceHistoryByProductIdAsync(productId), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(currency), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnPriceHistories_WhenOnlyProductIdIsProvided()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var priceHistories = _fixture
                        .Build<PriceHistory>()
                        .With(p => p.ProductId, productId)
                        .With(ph => ph.Product,
                            _fixture.Build<Product>()
                            .With(p => p.Id, productId)
                            .Create())
                        .CreateMany(5)
                        .ToList();

        var query = new GetPriceHistoryByProductIdQuery(productId);

        _priceHistoryRepositoryMock
            .Setup(repo => repo.GetPriceHistoryByProductIdAsync(productId))
            .ReturnsAsync(priceHistories);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(
            priceHistoryResponse => priceHistoryResponse,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().HaveSameCount(priceHistories);
        resultValue.Should().AllSatisfy(response =>
        {
            var originalPriceHistory = priceHistories.First(p => p.Id == response.Id);
            response.ProductId.Should().Be(productId);
            response.Currency.Should().BeNull();
            response.OldPrice.Should().Be(originalPriceHistory.OldPrice);
            response.NewPrice.Should().Be(originalPriceHistory.NewPrice);
        });

        _priceHistoryRepositoryMock.Verify(repo => repo.GetPriceHistoryByProductIdAsync(productId), Times.Once);
        _exchangeRateApiServiceMock.Verify(service => service.GetExchangeRateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPriceHistoriesFoundForProductId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetPriceHistoryByProductIdQuery(productId);

        _priceHistoryRepositoryMock
            .Setup(repo => repo.GetPriceHistoryByProductIdAsync(productId))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(
            priceHistoryResponse => priceHistoryResponse,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().BeEmpty();
        _priceHistoryRepositoryMock.Verify(repo => repo.GetPriceHistoryByProductIdAsync(productId), Times.Once);
    }
}