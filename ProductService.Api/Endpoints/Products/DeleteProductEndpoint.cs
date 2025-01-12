using FastEndpoints;
using MediatR;
using ProductService.Application.Products.DeleteProducts;

namespace ProductService.Api.Endpoints.Products;
public class DeleteProductEndpoint(IMediator mediator) : Endpoint<DeleteProductCommand>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Delete("/products/{id}");
        AllowAnonymous(); //For now
    }

    public override async Task HandleAsync(DeleteProductCommand req, CancellationToken ct)
    {
        await _mediator.Send(req, ct);
        await SendOkAsync(ct);
    }
}