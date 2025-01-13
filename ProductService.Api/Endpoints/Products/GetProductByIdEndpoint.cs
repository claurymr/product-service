using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;

namespace ProductService.Api.Endpoints.Products;
public class GetProductByIdEndpoint(IMediator mediator) : Endpoint<GetProductByIdQuery, Results<Ok<ProductResponse>, NotFound>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/products/{id}");
        AllowAnonymous(); //For now
    }

    public override async Task<Results<Ok<ProductResponse>, NotFound>> HandleAsync(GetProductByIdQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        var response = result.Match<IResult>(
                        productResponse => TypedResults.Ok(productResponse),
                        notFound => TypedResults.NotFound(notFound));
        return response switch
        {
            Ok<ProductResponse> success => success,
            NotFound notFound => notFound,
            _ => throw new Exception()
        };
    }
}