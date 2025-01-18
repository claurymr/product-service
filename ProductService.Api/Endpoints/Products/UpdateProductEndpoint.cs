using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.UpdateProducts;

namespace ProductService.Api.Endpoints.Products;

/// <summary>
/// Endpoint for updating a product.
/// </summary>
/// <param name="mediator">The mediator instance for sending commands.</param>
/// <response code="204">No Content - The product was successfully updated.</response>
/// <response code="400">Bad Request - The request is invalid, containing validation errors.</response>
/// <response code="401">Unauthorized - The request is not authorized.</response>
/// <response code="403">Forbidden - The request is forbidden.</response>
/// <response code="404">Not Found - The product with the specified ID was not found.</response>
public class UpdateProductEndpoint(IMediator mediator) 
    : Endpoint<UpdateProductCommand, Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.PUT);
        Put("/products/{id}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOnly");
            x.WithDisplayName("Update Product");
            x.Produces<NoContent>(StatusCodes.Status204NoContent);
            x.Produces<BadRequest<ValidationFailureResponse>>(StatusCodes.Status400BadRequest);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Accepts<UpdateProductCommand>();
            x.WithOpenApi();
        });
    }

    /// <summary>
    /// Executes the update product command asynchronously.
    /// </summary>
    /// <param name="req">The update product command request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the update operation.</returns>
    /// <exception cref="Exception">Thrown when an unexpected result is encountered.</exception>
    public override async Task<Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>> 
        ExecuteAsync(UpdateProductCommand req, CancellationToken ct)
    {
        var newReq = req with { Id = Route<Guid>("id") };

        var result = await _mediator.Send(newReq, ct);
        var response = result
            .Match<IResult>(
            noContent => TypedResults.NoContent(),
            failed => TypedResults.BadRequest(failed.MapToResponse()),
            notFound => TypedResults.NotFound<OperationFailureResponse>(notFound.MapToResponse())
        );
        return response switch
        {
            NoContent noContent => noContent,
            BadRequest<ValidationFailureResponse> badRequest => badRequest,
            NotFound<OperationFailureResponse> notFound => notFound,
            _ => throw new Exception()
        };
    }
}