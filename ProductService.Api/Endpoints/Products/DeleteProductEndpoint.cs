using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.DeleteProducts;

namespace ProductService.Api.Endpoints.Products;
public class DeleteProductEndpoint(IMediator mediator) : Endpoint<DeleteProductCommand, Results<NoContent, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Delete("/products/{id}");

        Options(x =>
        {
            x.RequireAuthorization("Admin");
            x.WithDisplayName("Delete Product");
            x.Produces<NoContent>(StatusCodes.Status204NoContent);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Accepts<DeleteProductCommand>();
            x.WithOpenApi();
        });
    }

    public override async Task<Results<NoContent, NotFound<OperationFailureResponse>>> ExecuteAsync(DeleteProductCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result.Match<IResult>(
                        guid => TypedResults.NoContent(),
                        notFound => TypedResults.NotFound(notFound.MapToResponse()));
        return response switch
        {
            NoContent noContent => noContent,
            NotFound<OperationFailureResponse> notFound => notFound,
            _ => throw new Exception()
        };
    }
}