using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.DeleteProducts;

namespace ProductService.Api.Endpoints.Products;

/// <summary>
/// Endpoint for deleting a product.
/// </summary>
/// <param name="mediator">The mediator instance for sending commands.</param>
/// <response code="204">No content, indicating the product was successfully deleted.</response>
/// <response code="404">Not found, indicating the product with the specified ID does not exist.</response>
/// <response code="401">Unauthorized, indicating the user is not authenticated.</response>
/// <response code="403">Forbidden, indicating the user does not have the necessary permissions.</response>
public class DeleteProductEndpoint(IMediator mediator)
    : Endpoint<DeleteProductCommand, Results<NoContent, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.DELETE);
        Delete("/products/{id}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOnly");
            x.WithDisplayName("Delete Product");
            x.Produces<NoContent>(StatusCodes.Status204NoContent);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Accepts<DeleteProductCommand>();
            x.WithOpenApi();
        });
    }

    /// <summary>
    /// Executes the delete product command asynchronously.
    /// </summary>
    /// <param name="req">The delete product command containing the product ID.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the delete operation, which can be NoContent or NotFound.</returns>
    /// <exception cref="Exception">Thrown when an unexpected result is encountered.</exception>
    public override async Task<Results<NoContent, NotFound<OperationFailureResponse>>>
        ExecuteAsync(DeleteProductCommand req, CancellationToken ct)
    {
        var newReq = req with { Id = Route<Guid>("id") };

        var result = await _mediator.Send(newReq, ct);
        var response = result.Match<IResult>(
                        guid => TypedResults.NoContent(),
                        notFound => TypedResults.NotFound(notFound.MapToResponse()));
        return response switch
        {
            NoContent noContent => noContent,
            NotFound<OperationFailureResponse> notFound => notFound,
            _ => throw new Exception()
        };
    }
}