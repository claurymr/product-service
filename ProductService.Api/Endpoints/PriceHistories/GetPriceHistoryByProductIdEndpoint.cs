using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;

namespace ProductService.Api.Endpoints.PriceHistories;
public class GetPriceHistoryByProductIdEndpoint(IMediator mediator)
    : Endpoint<GetPriceHistoryByProductIdQuery, Results<Ok<IEnumerable<PriceHistoryResponse>>, JsonHttpResult<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.GET);
        Get("/pricehistories/{productId}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOrUser");
            x.WithDisplayName("Get Price Histories by Product Id");
            x.Produces<Ok<IEnumerable<PriceHistoryResponse>>>(StatusCodes.Status200OK);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Accepts<GetPriceHistoryByProductIdQuery>();
            x.WithOpenApi();
        });
    }

    public override async Task<Results<Ok<IEnumerable<PriceHistoryResponse>>, JsonHttpResult<OperationFailureResponse>>>
        ExecuteAsync(GetPriceHistoryByProductIdQuery req, CancellationToken ct)
    {
        var newReq = req with { ProductId = Route<Guid>("productId") };

        var result = await _mediator.Send(newReq, ct);
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