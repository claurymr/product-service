using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Application.Validation;

namespace ProductService.Api.Endpoints.Products;
public class UpdateProductEndpoint(IMediator mediator) 
    : Endpoint<UpdateProductCommand, Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Put("/products/{id}");
        AllowAnonymous(); //For now
    }

    public override async Task<Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>> 
        ExecuteAsync(UpdateProductCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result
            .Match<IResult>(
            noContent => TypedResults.NoContent(),
            failed => TypedResults.BadRequest(failed.MapToResponse()),
            notFound => TypedResults.NotFound<OperationFailureResponse>(notFound.MapToResponse())
        );
        return response switch
        {
            NoContent noContent => noContent,
            BadRequest<ValidationFailureResponse> badRequest => badRequest,
            NotFound<OperationFailureResponse> notFound => notFound,
            _ => throw new Exception()
        };
    }
}