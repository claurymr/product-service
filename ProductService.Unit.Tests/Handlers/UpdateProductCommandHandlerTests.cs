using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Domain;
using ProductService.Infrastructure.Handlers.Products.UpdateProducts;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class UpdateProductCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IValidator<UpdateProductCommand>> _validatorMock;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _validatorMock = _fixture.Freeze<Mock<IValidator<UpdateProductCommand>>>();
        _handler = new UpdateProductCommandHandler(_productRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductAndReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = _fixture.Build<UpdateProductCommand>()
                              .With(c => c.Id, productId)
                              .Create();

        _validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _productRepositoryMock
            .Setup(repo => repo.UpdateProductAsync(It.IsAny<Guid>(), It.IsAny<Product>()))
            .ReturnsAsync(productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(guid => guid, _ => default!, _ => default!);
        resultValue.Should().Be(productId);
        _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(productId, It.IsAny<Product>()), Times.Once);
        // verify event publisher is called once
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenRequestIsInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = _fixture.Build<UpdateProductCommand>()
                              .With(c => c.Id, productId)
                              .Without(p => p.Name)
                              .Without(p => p.Price)
                              .Create();
        var validationErrors = new ValidationResult(new List<ValidationFailure>
        {
            new("Name", "Name is required."),
            new("Price", "Price is required.")
        });
        _validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationErrors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultValidation = result.Match(
            _ => default!,
            validation => validation.MapToResponse(),
            _ => default!);
        resultValidation.Should().NotBeNull();
        resultValidation.Should().BeOfType<ValidationFailureResponse>();
        resultValidation.Errors.Should().HaveCount(2);
        resultValidation.Errors.Should().ContainSingle(e => e.Message == "Name is required.");
        resultValidation.Errors.Should().ContainSingle(e => e.Message == "Price is required.");

        _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(It.IsAny<Guid>(), It.IsAny<Product>()), Times.Never);
        // verify event publisher is not called
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
        result.IsSuccess.Should().BeFalse();
        var resultNotFound = result.Match(
            _ => default!,
            _ => default!,
            notFound => notFound.MapToResponse());
        resultNotFound.Should().NotBeNull();
        resultNotFound.Should().BeOfType<OperationFailureResponse>();
        resultNotFound.Errors.Should().ContainSingle(e => e.Message == $"Product with Id {productId} not found.");

        _productRepositoryMock.Verify(repo => repo.UpdateProductAsync(productId, It.IsAny<Product>()), Times.Once);
        // verify event publisher is not called
    }
}