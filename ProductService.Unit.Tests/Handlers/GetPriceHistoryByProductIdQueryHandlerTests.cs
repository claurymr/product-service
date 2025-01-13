using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Contracts;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;
using ProductService.Application.Repositories;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class GetPriceHistoryByProductIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IPriceHistoryRepository> _priceHistoryRepositoryMock;
    private readonly GetPriceHistoryByProductIdQueryHandler _handler;

    public GetPriceHistoryByProductIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _priceHistoryRepositoryMock = _fixture.Freeze<Mock<IPriceHistoryRepository>>();
        _handler = new GetPriceHistoryByProductIdQueryHandler(_priceHistoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPriceHistories_WhenProductIdIsProvided()
    {
        // Arrange
        var priceHistoryResponses = _fixture
                                    .CreateMany<PriceHistoryResponse>()
                                    .ToList();
        var productId = Guid.NewGuid();

        var query = new GetPriceHistoryByProductIdQuery(productId);

        _priceHistoryRepositoryMock
            .Setup(repo => repo.GetPriceHistoryByProductIdAsync(productId))
            .ReturnsAsync(priceHistoryResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveSameCount(priceHistoryResponses);
        result.Should().BeEquivalentTo(priceHistoryResponses);
        _priceHistoryRepositoryMock.Verify(repo => repo.GetPriceHistoryByProductIdAsync(productId), Times.Once);
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
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _priceHistoryRepositoryMock.Verify(repo => repo.GetPriceHistoryByProductIdAsync(productId), Times.Once);
    }
}