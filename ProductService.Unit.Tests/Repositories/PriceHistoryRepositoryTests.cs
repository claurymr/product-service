using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using ProductService.Domain;
using ProductService.Domain.Enums;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using ProductService.Unit.Tests.DbContextBuild;
using Xunit;

namespace ProductService.Unit.Tests.Repositories;
public class PriceHistoryRepositoryTests
{
    private readonly IFixture _fixture;
    private readonly ProductServiceDbContext _dbContextMock;

    public PriceHistoryRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _dbContextMock = DbContextFactory.CreateInMemoryDbContext();
    }

    [Fact]
    public async Task GetPriceHistoryByProductIdAsync_ShouldReturnPriceHistory_WhenProductIdExists()
    {
        // Arrange
        var productAdd = _fixture
                            .Build<Product>()
                            .With(p => p.Id, Guid.Empty)
                            .With(p => p.Price, 200)
                            .Create();
        var repository = new PriceHistoryRepository(_dbContextMock);
        var result = await _dbContextMock.Products.AddAsync(productAdd);
        var expectedHistory1 = _fixture
                                .Build<PriceHistory>()
                                .With(p => p.ProductId, productAdd.Id)
                                .With(p => p.Product, productAdd)
                                .With(p => p.OldPrice, 0)
                                .With(p => p.NewPrice, productAdd.Price)
                                .With(p => p.Action, ActionType.Entry)
                                .With(p => p.Timestamp, DateTime.UtcNow.AddDays(-1))
                                .Create();
        productAdd.Price = 100;
        var expectedHistory2 = _fixture
                                .Build<PriceHistory>()
                                .With(p => p.ProductId, productAdd.Id)
                                .With(p => p.Product, productAdd)
                                .With(p => p.OldPrice, expectedHistory1.NewPrice)
                                .With(p => p.NewPrice, productAdd.Price)
                                .With(p => p.Action, ActionType.Reduced)
                                .With(p => p.Timestamp, DateTime.UtcNow)
                                .Create();
        await _dbContextMock.PriceHistories.AddRangeAsync(expectedHistory1, expectedHistory2);
        await _dbContextMock.SaveChangesAsync();

        // Act
        var histories = await repository.GetPriceHistoryByProductIdAsync(productAdd.Id);

        // Assert
        histories.Should().HaveCount(2);
        histories.Should().AllSatisfy(c => c.ProductId = productAdd.Id);
    }

    [Fact]
    public async Task GetPriceHistoryByProductIdAsync_ShouldReturnEmpty_WhenProductIdDoesNotExist()
    {
        // Arrange
        var productAdd = _fixture
                            .Build<Product>()
                            .With(p => p.Id, Guid.Empty)
                            .With(p => p.Price, 200)
                            .Create();
        var repository = new PriceHistoryRepository(_dbContextMock);
        var result = await _dbContextMock.Products.AddAsync(productAdd);
        var addedProductId = result.Entity.Id;
        var expectedHistory1 = _fixture
                                .Build<PriceHistory>()
                                .With(p => p.OldPrice, 0)
                                .With(p => p.NewPrice, productAdd.Price)
                                .With(p => p.Action, ActionType.Entry)
                                .With(p => p.Timestamp, DateTime.UtcNow.AddDays(-1))
                                .Create();
        productAdd.Price = 100;
        var expectedHistory2 = _fixture
                                .Build<PriceHistory>()
                                .With(p => p.OldPrice, expectedHistory1.NewPrice)
                                .With(p => p.NewPrice, productAdd.Price)
                                .With(p => p.Action, ActionType.Entry)
                                .With(p => p.Timestamp, DateTime.UtcNow)
                                .Create();
        await _dbContextMock.PriceHistories.AddRangeAsync(expectedHistory1, expectedHistory2);
        await _dbContextMock.SaveChangesAsync();

        // Act
        var histories = await repository.GetPriceHistoryByProductIdAsync(addedProductId);

        // Assert
        histories.Should().BeEmpty();
    }
}