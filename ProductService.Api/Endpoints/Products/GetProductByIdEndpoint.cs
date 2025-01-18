using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;

/// <summary>
/// Endpoint for retrieving a product by its ID.
/// </summary>
/// <param name="mediator">The mediator instance for handling requests.</param>
/// <response code="200">Returns the product details if found.</response>
/// <response code="500">Returns an internal server error if there is a failure.</response>
/// <response code="404">Returns not found if the product does not exist.</response>
/// <response code="401">Returns unauthorized if the user is not authenticated.</response>
/// <response code="403">Returns forbidden if the user does not have the required permissions.</response>
public class GetProductByIdEndpoint(IMediator mediator)
    : Endpoint<GetProductByIdQuery, Results<Ok<ProductResponse>, JsonHttpResult<OperationFailureResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.GET);
        Get("/products/{id}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOrUser");
            x.WithDisplayName("Get Product by Id");
            x.Produces<Ok<ProductResponse>>(StatusCodes.Status200OK);
            x.Produces<JsonHttpResult<OperationFailureResponse>>(StatusCodes.Status500InternalServerError);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Accepts<GetProductByIdQuery>();
            x.WithOpenApi();
        });
    }

    /// <summary>
    /// Executes the request to get a product by its ID.
    /// </summary>
    /// <param name="req">The request containing the product ID.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    public override async Task<Results<Ok<ProductResponse>, JsonHttpResult<OperationFailureResponse>, NotFound<OperationFailureResponse>>>
        ExecuteAsync(GetProductByIdQuery req, CancellationToken ct)
    {
        var newReq = req with { Id = Route<Guid>("id") };

        var result = await _mediator.Send(newReq, ct);
        var response = result.Match<IResult>(
                        productResponse => TypedResults.Ok(productResponse),
                        failed => TypedResults.Json(
                                    failed.MapToResponse(),
                                    statusCode: StatusCodes.Status500InternalServerError),
                        notFound => TypedResults.NotFound(notFound.MapToResponse()));
        return response switch
        {
            Ok<ProductResponse> success => success,
            JsonHttpResult<OperationFailureResponse> problem => problem,
            NotFound<OperationFailureResponse> notFound => notFound,
            _ => throw new Exception()
        };
    }
}