using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Mappings;
using ProductService.Application.Products.DeleteProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Infrastructure.Handlers.Products.DeleteProducts;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class DeleteProductCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _handler = new DeleteProductCommandHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepositoryMock
            .Setup(repo => repo.DeleteProductAsync(productId))
            .ReturnsAsync(productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(guid => guid, _ => default!);
        resultValue.Should().Be(productId);
        _productRepositoryMock.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);
        // verify publisher is called once
    }

    [Fact]
    public async Task Handle_ShouldReturnRecordNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepositoryMock
            .Setup(repo => repo.DeleteProductAsync(productId))
            .ReturnsAsync(Guid.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultNotFound = result.Match(
            _ => default!, 
            notFound => notFound.MapToResponse());
        resultNotFound.Should().NotBeNull();
        resultNotFound.Should().BeOfType<RecordNotFound>();
        resultNotFound.Errors.Should().ContainSingle(e => e.Message == $"Product with ID {productId} not found.");
        _productRepositoryMock.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);
        // verify publisher is not called
    }
}