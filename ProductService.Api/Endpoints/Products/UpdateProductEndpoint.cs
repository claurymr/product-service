using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.UpdateProducts;

namespace ProductService.Api.Endpoints.Products;
public class UpdateProductEndpoint(IMediator mediator) 
    : Endpoint<UpdateProductCommand, Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.PUT);
        Put("/products/{id}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOnly");
            x.WithDisplayName("Update Product");
            x.Produces<NoContent>(StatusCodes.Status204NoContent);
            x.Produces<BadRequest<ValidationFailureResponse>>(StatusCodes.Status400BadRequest);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Accepts<UpdateProductCommand>();
            x.WithOpenApi();
        });
    }

    public override async Task<Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>> 
        ExecuteAsync(UpdateProductCommand req, CancellationToken ct)
    {
        var newReq = req with { Id = Route<Guid>("id") };

        var result = await _mediator.Send(newReq, ct);
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