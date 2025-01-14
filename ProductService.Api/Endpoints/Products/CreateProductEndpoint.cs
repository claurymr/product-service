using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;

namespace ProductService.Api.Endpoints.Products;
public class CreateProductEndpoint(IMediator mediator)
    : Endpoint<CreateProductCommand, Results<Created<Guid>, BadRequest<ValidationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("/products");
        AllowAnonymous(); //For now
    }

    public override async Task<Results<Created<Guid>, BadRequest<ValidationFailureResponse>>>
        ExecuteAsync(CreateProductCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result.Match<IResult>(
            guid => TypedResults.Created($"/products/{guid}", guid),
            failed => TypedResults.BadRequest(failed.MapToResponse()));
        return response switch
        {
            Created<Guid> success => success,
            BadRequest<ValidationFailureResponse> badRequest => badRequest,
            _ => throw new Exception()
        };
    }
}