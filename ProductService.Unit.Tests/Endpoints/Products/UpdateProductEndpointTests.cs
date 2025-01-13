using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProductService.Api.Endpoints.Products;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Application.Validation;
using Xunit;

namespace ProductService.Unit.Tests.Endpoints.Products;
public class UpdateProductEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UpdateProductEndpoint _endpoint;

    public UpdateProductEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new UpdateProductEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNoContent_WhenProductIsUpdated()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<UpdateProductCommand>()
                        .With(p => p.Id, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NoContent));
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNotFound_WhenProductIsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<UpdateProductCommand>()
                        .With(p => p.Id, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(Guid));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NotFound));
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = _fixture
                        .Build<UpdateProductCommand>()
                        .Without(p => p.Name)
                        .Create();
        var validationFailed = new ValidationFailed(new ValidationFailure(nameof(UpdateProductCommand.Name), "Name is required"));

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationFailed);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(BadRequest));
    }
}