using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProductService.Api.Endpoints.Products;
using ProductService.Application.Contracts;
using ProductService.Application.Products.DeleteProducts;
using ProductService.Application.Validation;
using Xunit;

namespace ProductService.Unit.Tests.Endpoints.Products;
public class DeleteProductEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeleteProductEndpoint _endpoint;

    public DeleteProductEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new DeleteProductEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenProductIsDeleted()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<DeleteProductCommand>()
                        .With(p => p.Id, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NoContent));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFound_WhenProductIsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<DeleteProductCommand>()
                        .With(p => p.Id, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecordNotFound([$"Product with Id {request.Id} not found."]));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NotFound<OperationFailureResponse>));
        (result.Result as NotFound<OperationFailureResponse>)!.Value!
            .Errors.Should().Contain(c => c.Message == $"Product with Id {request.Id} not found.");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}