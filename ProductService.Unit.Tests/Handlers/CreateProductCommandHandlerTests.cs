using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Domain;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class CreateProductCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public CreateProductCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProductAndReturnId_WhenRequestIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = _fixture.Create<CreateProductCommand>();

        _productRepositoryMock
            .Setup(repo => repo.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(productId);

        // for later: mock triggering an event

        var handler = new CreateProductCommandHandler(_productRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(guid => guid, _ => default!);
        resultValue.Should().Be(productId);
        result.IsError.Should().BeFalse();
        _productRepositoryMock.Verify(
            repo => repo.CreateProductAsync(It.Is<Product>
                (p =>
                    p.Name == command.Name &&
                    p.Description == command.Description &&
                    p.Price == command.Price &&
                    p.Category == command.Category &&
                    p.Sku == command.Sku)),
            Times.Once);

        // for later: verify trigger of event
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationFailed_WhenValidationFails()
    {
        // Arrange
        var command = _fixture.Build<CreateProductCommand>()
                              .With(c => c.Name, string.Empty) // Simulate invalid data
                              .Create();
        var handler = new CreateProductCommandHandler(_productRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var resultError = result.Match(
            _ => default!,
            failed => failed.MapToResponse());
        resultError.Should().NotBeNull();
        resultError.Should().BeOfType<ValidationFailed>();

        _productRepositoryMock.Verify(
            repo => repo.CreateProductAsync(It.IsAny<Product>()),
            Times.Never);
    }
}