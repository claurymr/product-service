using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProductService.Api.Endpoints.Products;
using ProductService.Application.Contracts;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Validation;
using Xunit;

namespace ProductService.Unit.Tests.Endpoints.Products;
public class CreateProductEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CreateProductEndpoint _endpoint;

    public CreateProductEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new CreateProductEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedProduct_WhenRequestIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture.Create<CreateProductCommand>();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Created<Guid>));
        (result.Result as Created<Guid>)!.Value.Should().Be(productId);

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = _fixture
                        .Build<CreateProductCommand>()
                        .Without(p => p.Name)
                        .Create();
        var validationFailed = new ValidationFailed(new ValidationFailure(nameof(CreateProductCommand.Name), "Name is required"));

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationFailed);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(BadRequest<ValidationFailureResponse>));
        (result.Result as BadRequest<ValidationFailureResponse>)!.Value!
            .Errors.Should().HaveCount(1);
        (result.Result as BadRequest<ValidationFailureResponse>)!.Value!
        .Errors.Should().ContainSingle(c => c.Message == "Name is required");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}