using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using ProductService.Unit.Tests.DbContextBuild;
using Xunit;

namespace ProductService.Unit.Tests.Repositories;
public class ProductRepositoryTests
{
    private readonly IFixture _fixture;
    private readonly ProductServiceDbContext _dbContextMock;

    public ProductRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _dbContextMock = DbContextFactory.CreateInMemoryDbContext();
    }

    [Fact]
    public async Task CreateProductAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        var productAdd = _fixture
                            .Build<Product>()
                            .With(p => p.Id, Guid.Empty)
                            .Create();
        var repository = new ProductRepository(_dbContextMock);
        var result = await _dbContextMock.Products.AddAsync(productAdd);
        await _dbContextMock.SaveChangesAsync();

        // Act
        var entityId = await repository.CreateProductAsync(productAdd);

        // Assert
        entityId.Should().NotBeEmpty();
        _dbContextMock.Products.Should().ContainSingle(p => p.Id == entityId);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDeleteProductFromDatabase()
    {
        // Arrange
        var productAdd1 = _fixture
                            .Build<Product>()
                            .With(p => p.Id, Guid.Empty)
                            .Create();
        var productAdd2 = _fixture
                            .Build<Product>()
                            .With(p => p.Id, Guid.Empty)
                            .Create();
        var repository = new ProductRepository(_dbContextMock);
        await _dbContextMock.Products.AddRangeAsync(productAdd1, productAdd2);
        await _dbContextMock.SaveChangesAsync();
        Guid productIdToDelete = (await _dbContextMock.Products.FirstOrDefaultAsync())!.Id;

        // Act
        await repository.DeleteProductAsync(productIdToDelete);

        // Assert
        _dbContextMock.Products.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldNotDeleteProductFromDatabase_WhenProductDoesNotExist()
    {
        // Arrange
        var productAdd = _fixture
                            .Build<Product>()
                            .With(p => p.Id, Guid.Empty)
                            .Create();
        var repository = new ProductRepository(_dbContextMock);
        await _dbContextMock.Products.AddAsync(productAdd);
        await _dbContextMock.SaveChangesAsync();
        Guid productIdToDelete = _fixture.Create<Guid>();

        // Act
        await repository.DeleteProductAsync(productIdToDelete);

        // Assert
        _dbContextMock.Products.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = _fixture.Build<Product>()
                        .With(p => p.Id, Guid.Empty)
                        .CreateMany(5).ToList();
        await _dbContextMock.Products.AddRangeAsync(products);
        await _dbContextMock.SaveChangesAsync();

        var repository = new ProductRepository(_dbContextMock);

        // Act
        var result = await repository.GetProductsAsync();

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeEquivalentTo(products, options => options.Excluding(p => p.Id));
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnProductsInCategory()
    {
        // Arrange
        var category = "Electronics";
        var productsInCategory = _fixture
                                .Build<Product>()
                                .With(p => p.Id, Guid.Empty)
                                .With(p => p.Category, category)
                                .CreateMany(3)
                                .ToList();
        var otherProducts = _fixture
                            .Build<Product>()
                            .With(p => p.Id, Guid.Empty)
                            .With(p => p.Category, "Other")
                            .CreateMany(2)
                            .ToList();

        await _dbContextMock.Products.AddRangeAsync(productsInCategory.Concat(otherProducts));
        await _dbContextMock.SaveChangesAsync();

        var repository = new ProductRepository(_dbContextMock);

        // Act
        var result = await repository.GetProductsByCategory(category);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(p => p.Category.Should().Be(category));
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = _fixture
                        .Build<Product>()
                        .With(p => p.Id, Guid.Empty)
                        .Create();
        await _dbContextMock.Products.AddAsync(product);
        await _dbContextMock.SaveChangesAsync();

        var repository = new ProductRepository(_dbContextMock);

        // Act
        var result = await repository.GetProductByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(product, options => options.Excluding(p => p.Id));
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnEmpty_WhenProductDoesNotExist()
    {
        // Arrange
        var products = _fixture.Build<Product>()
                        .With(p => p.Id, Guid.Empty)
                        .CreateMany(5).ToList();
        await _dbContextMock.Products.AddRangeAsync(products);
        await _dbContextMock.SaveChangesAsync();
        var nonExistentProductId = Guid.NewGuid();

        var repository = new ProductRepository(_dbContextMock);

        // Act
        var result = await repository.GetProductByIdAsync(nonExistentProductId);

        // Assert
        result.Should().BeNull();
    }
}