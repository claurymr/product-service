using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Mappings;
using ProductService.Application.Products.UpdateProducts;

namespace ProductService.Api.Endpoints.Products;
public class UpdateProductEndpoint(IMediator mediator) : Endpoint<UpdateProductCommand, Results<Ok, BadRequest, NotFound>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Put("/products/{id}");
        AllowAnonymous(); //For now
    }

    public override async Task<Results<Ok, BadRequest, NotFound>> ExecuteAsync(UpdateProductCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result.Match<IResult>(
            _ => TypedResults.Ok(),
            failed => TypedResults.BadRequest(failed.MapToResponse()),
            notFound => TypedResults.NotFound(notFound.MapToResponse()));

        return response switch
        {
            Ok ok => ok,
            BadRequest badRequest => badRequest,
            _ => throw new Exception()
        };
    }
}