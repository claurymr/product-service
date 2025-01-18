using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ProductService.Application.Contracts;
using ProductService.Application.EventBus;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Repositories;
using ProductService.Domain;
using ProductService.Domain.Enums;
using ProductService.Infrastructure.Handlers.Products.CreateProducts;
using Shared.Contracts.Events;
using Xunit;

namespace ProductService.Unit.Tests.Handlers;
public class CreateProductCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IPriceHistoryRepository> _priceHistoryRepositoryMock;
    private readonly Mock<IValidator<CreateProductCommand>> _validatorMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRepositoryMock = _fixture.Freeze<Mock<IProductRepository>>();
        _priceHistoryRepositoryMock = _fixture.Freeze<Mock<IPriceHistoryRepository>>();
        _validatorMock = _fixture.Freeze<Mock<IValidator<CreateProductCommand>>>();
        _eventBusMock = _fixture.Freeze<Mock<IEventBus>>();
        _handler = new CreateProductCommandHandler(_productRepositoryMock.Object, 
                    _priceHistoryRepositoryMock.Object, 
                    _validatorMock.Object,
                    _eventBusMock.Object);
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
        _priceHistoryRepositoryMock
            .Setup(repo => repo.CreatePriceHistoryByProductIdAsync(It.IsAny<Guid>(), 
                It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<ActionType>()))
            .Returns(Task.CompletedTask);
        _eventBusMock
            .Setup(eventBus => eventBus.PublishAsync(It.IsAny<ProductCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result.Match(guid => guid, _ => default!);
        resultValue.Should().Be(productId);
        result.IsError.Should().BeFalse();
        _productRepositoryMock.Verify(
            repo => repo.CreateProductAsync(It.IsAny<Product>()), Times.Once);
        _priceHistoryRepositoryMock.Verify(
            repo => repo.CreatePriceHistoryByProductIdAsync(productId, 0m, command.Price, ActionType.Entry), Times.Once);
        _eventBusMock.Verify(
            eventBus => eventBus.PublishAsync(It.IsAny<ProductCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
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