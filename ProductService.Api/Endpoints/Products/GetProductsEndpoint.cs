using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;
public class GetProductsEndpoint(IMediator mediator)
    : Endpoint<GetAllProductsQuery, Results<Ok<IEnumerable<ProductResponse>>, JsonHttpResult<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/products");
        AllowAnonymous(); //For now
    }

    public override async Task<Results<Ok<IEnumerable<ProductResponse>>, JsonHttpResult<OperationFailureResponse>>>
        ExecuteAsync(GetAllProductsQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
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