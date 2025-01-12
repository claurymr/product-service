using FastEndpoints;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;
public class GetProductsByCategoryEndpoint(IMediator mediator) : Endpoint<GetProductsByCategoryQuery, IEnumerable<ProductResponse>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/products");
        AllowAnonymous(); //For now
    }

    public override async Task HandleAsync(GetProductsByCategoryQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(result!, ct);
    }
}