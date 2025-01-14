using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Domain;
using ProductService.Infrastructure.Handlers.Products.CreateProducts;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class CreateProductCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IValidator<CreateProductCommand>> _validatorMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _validatorMock = _fixture.Freeze<Mock<IValidator<CreateProductCommand>>>();
        _handler = new CreateProductCommandHandler(_productRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateProductAndReturnId_WhenRequestIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = _fixture.Create<CreateProductCommand>();

        _validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _productRepositoryMock
            .Setup(repo => repo.CreateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(productId);

        // for later: mock triggering an event

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

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

        var validationFailures = new List<ValidationFailure>{
                new("Name", "Name is required")
            };
        _validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var resultError = result.Match(
            _ => default!,
            failed => failed.MapToResponse());
        resultError.Should().NotBeNull();
        resultError.Should().BeOfType <ValidationFailureResponse>();

        _productRepositoryMock.Verify(
            repo => repo.CreateProductAsync(It.IsAny<Product>()),
            Times.Never);
    }
}