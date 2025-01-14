using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Mappings;

namespace ProductService.Infrastructure.Handlers.Products.GetProducts;
public class GetProductsByCategoryQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductResponse>>
{
    // Declare httpclient private field
    public async Task<IEnumerable<ProductResponse>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        var products = await productRepository.GetProductsByCategoryAsync(request.Category);
        // get products by category with applied calculation of price, and mapped to currency
        return products.MapToResponse();
    }
}