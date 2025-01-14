using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Application.Mappings;

namespace ProductService.Infrastructure.Handlers.Products.GetProducts;
public class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse, RecordNotFound>>
{
    // Declare httpclient private field
    public async Task<Result<ProductResponse, RecordNotFound>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        var product = await productRepository.GetProductByIdAsync(request.Id);
        if(product is null)
        {
            return new RecordNotFound([$"Product with Id {request.Id} not found."]);
        }
        // get products by id with applied calculation of price, and mapped to currency
        return product.MapToResponse();
    }
}