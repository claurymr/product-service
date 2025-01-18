using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;

namespace ProductService.Api.Endpoints.Products;

/// <summary>
/// Endpoint for creating a new product.
/// </summary>
/// <param name="mediator">The mediator instance for sending commands.</param>
/// <response code="201">Returns the GUID of the created product.</response>
/// <response code="400">Returns a validation failure response if the request is invalid.</response>
/// <response code="401">Returns an unauthorized response if the user is not authenticated.</response>
/// <response code="403">Returns a forbidden response if the user does not have the required permissions.</response>
public class CreateProductEndpoint(IMediator mediator)
    : Endpoint<CreateProductCommand, Results<Created<Guid>, BadRequest<ValidationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.POST);
        Post("/products");
        
        Options(x =>
        {
            x.RequireAuthorization("AdminOnly");
            x.WithDisplayName("Create Product");
            x.Produces<Created<Guid>>(StatusCodes.Status201Created);
            x.Produces<BadRequest<ValidationFailureResponse>>(StatusCodes.Status400BadRequest);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Accepts<CreateProductCommand>();
            x.WithOpenApi();
        });
    }

    /// <summary>
    /// Executes the command to create a product.
    /// </summary>
    /// <param name="req">The command containing the product creation details.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the product creation operation.</returns>
    /// <exception cref="Exception">Thrown when an unexpected result is encountered.</exception>
    public override async Task<Results<Created<Guid>, BadRequest<ValidationFailureResponse>>>
        ExecuteAsync(CreateProductCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result.Match<IResult>(
            guid => TypedResults.Created($"/products/{guid}", guid),
            failed => TypedResults.BadRequest(failed.MapToResponse()));
        return response switch
        {
            Created<Guid> success => success,
            BadRequest<ValidationFailureResponse> badRequest => badRequest,
            _ => throw new Exception()
        };
    }
}