using FastEndpoints;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.ProductPriceHistories.GetPriceHistories;

namespace ProductService.Api.Endpoints.PriceHistories;
public class GetPriceHistoryByProductIdEndpoint(IMediator mediator) : Endpoint<GetPriceHistoryByProductIdQuery, IEnumerable<PriceHistoryResponse>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/pricehistory/{productId}");
        AllowAnonymous(); //For now
    }

    public override async Task HandleAsync(GetPriceHistoryByProductIdQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await SendOkAsync(result is null ? [] : result!, ct);
    }
}