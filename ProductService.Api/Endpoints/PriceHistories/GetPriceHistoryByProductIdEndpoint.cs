using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;
using ProductService.Application.Validation;

namespace ProductService.Api.Endpoints.PriceHistories;
public class GetPriceHistoryByProductIdEndpoint(IMediator mediator)
    : Endpoint<GetPriceHistoryByProductIdQuery, Results<Ok<IEnumerable<PriceHistoryResponse>>, JsonHttpResult<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/pricehistory/{productId}");
        AllowAnonymous(); //For now
    }

    public override async Task<Results<Ok<IEnumerable<PriceHistoryResponse>>, JsonHttpResult<OperationFailureResponse>>> ExecuteAsync(GetPriceHistoryByProductIdQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result.Match<IResult>(
                        productsResponse => TypedResults.Ok(productsResponse),
                        failed =>
                        {
                            return TypedResults.Json(failed.MapToResponse(), statusCode: StatusCodes.Status500InternalServerError);
                        });
        return response switch
        {
            Ok<IEnumerable<PriceHistoryResponse>> success => success,
            JsonHttpResult<OperationFailureResponse> jsonHttpResult => jsonHttpResult,
            _ => throw new Exception()
        };
    }
}