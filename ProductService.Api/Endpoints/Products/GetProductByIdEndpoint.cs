using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;
public class GetProductByIdEndpoint(IMediator mediator)
    : Endpoint<GetProductByIdQuery, Results<Ok<ProductResponse>, JsonHttpResult<OperationFailureResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
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

    public override async Task<Results<Ok<ProductResponse>, JsonHttpResult<OperationFailureResponse>, NotFound<OperationFailureResponse>>>
        ExecuteAsync(GetProductByIdQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
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