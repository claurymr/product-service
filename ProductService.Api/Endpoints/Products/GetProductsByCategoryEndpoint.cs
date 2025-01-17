using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;
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