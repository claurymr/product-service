using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Mappings;
using ProductService.Application.Products.DeleteProducts;

namespace ProductService.Api.Endpoints.Products;
public class DeleteProductEndpoint(IMediator mediator) : Endpoint<DeleteProductCommand, Results<Ok<Guid>, NotFound>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Delete("/products/{id}");
        AllowAnonymous(); //For now
    }

    public override async Task<Results<Ok<Guid>, NotFound>> HandleAsync(DeleteProductCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result.Match<IResult>(
                        guid => TypedResults.Ok(guid),
                        notFound => TypedResults.NotFound(notFound.MapToResponse()));
        return response switch
        {
            Ok<Guid> success => success,
            NotFound notFound => notFound,
            _ => throw new Exception()
        };
    }
}