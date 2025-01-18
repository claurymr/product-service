using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;

/// <summary>
/// Endpoint to get products by category.
/// </summary>
/// <param name="mediator">The mediator instance for sending requests.</param>
/// <response code="200">Returns a list of products in the specified category.</response>
/// <response code="500">Returns an error response if the operation fails.</response>
/// <response code="401">Returns an unauthorized response if the user is not authenticated.</response>
/// <response code="403">Returns a forbidden response if the user does not have the required permissions.</response>
public class GetProductsByCategoryEndpoint(IMediator mediator)
    : Endpoint<GetProductsByCategoryQuery, Results<Ok<IEnumerable<ProductResponse>>, JsonHttpResult<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.GET);
        Get("/products/categories/{category}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOrUser");
            x.WithDisplayName("Get Products by Category");
            x.Produces<Ok<IEnumerable<ProductResponse>>>(StatusCodes.Status200OK);
            x.Produces<JsonHttpResult<OperationFailureResponse>>(StatusCodes.Status500InternalServerError);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Accepts<GetProductsByCategoryQuery>();
            x.WithOpenApi();
        });
    }

    /// <summary>
    /// Executes the request to get products by category.
    /// </summary>
    /// <param name="req">The request containing the category information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    /// <exception cref="Exception">Thrown when an unexpected result is encountered.</exception>
    public override async Task<Results<Ok<IEnumerable<ProductResponse>>, JsonHttpResult<OperationFailureResponse>>>
        ExecuteAsync(GetProductsByCategoryQuery req, CancellationToken ct)
    {
        var newReq = req with { Category = Route<string>("category")! };

        var result = await _mediator.Send(newReq, ct);
        var response = result.Match<IResult>(
                        productsResponse => TypedResults.Ok(productsResponse),
                        failed => TypedResults.Json(failed.MapToResponse(), statusCode: StatusCodes.Status500InternalServerError));
        return response switch
        {
            Ok<IEnumerable<ProductResponse>> success => success,
            JsonHttpResult<OperationFailureResponse> problem => problem,
            _ => throw new Exception()
        };
    }
}