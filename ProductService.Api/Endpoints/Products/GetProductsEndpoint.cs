using FastEndpoints;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;
public class GetProductsEndpoint(IMediator mediator) : Endpoint<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/products");
        AllowAnonymous(); //For now
    }

    public override async Task HandleAsync(GetAllProductsQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await SendOkAsync(result is null ? [] : result!, ct);
    }
}