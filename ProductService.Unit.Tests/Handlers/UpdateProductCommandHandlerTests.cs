using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application.Mappings;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Domain;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class UpdateProductCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _handler = new UpdateProductCommandHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductAndReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = _fixture.Build<UpdateProductCommand>()
                              .With(c => c.Id, productId)
                              .Create();

        _productRepositoryMock
            .Setup(repo => repo.UpdateProductAsync(It.IsAny<Guid>(), It.IsAny<Product>()))
            .ReturnsAsync(productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(guid => guid, _ => default!, _ => default!);
        resultValue.Should().Be(productId);
        _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(productId, It.Is<Product>
            (p =>
                p.Id == command.Id &&
                p.Name == command.Name &&
                p.Description == command.Description &&
                p.Price == command.Price &&
                p.Category == command.Category &&
                p.Sku == command.Sku)),
            Times.Once);
        // verify event publisher is called once
    }

    [Fact]
    public async Task Handle_ShouldReturnRecordNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = _fixture.Build<UpdateProductCommand>()
                              .With(c => c.Id, productId)
                              .Create();

        _productRepositoryMock
            .Setup(repo => repo.UpdateProductAsync(It.IsAny<Guid>(), It.IsAny<Product>()))
            .ReturnsAsync(Guid.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultNotFound = result.Match(
            _ => default!,
            _ => default!,
            notFound => notFound.MapToResponse());
        resultNotFound.Should().NotBeNull();
        resultNotFound.Should().BeOfType<RecordNotFound>();
        resultNotFound.Errors.Should().ContainSingle(e => e.Message == $"Product with ID {productId} not found.");
        _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(productId, It.IsAny<Product>()), Times.Once);
        // verify event publisher is not called
    }
}