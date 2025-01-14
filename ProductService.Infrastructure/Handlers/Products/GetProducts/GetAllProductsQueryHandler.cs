using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.GetProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Mappings;

namespace ProductService.Infrastructure.Handlers.Products.GetProducts;
public class GetAllProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    // Declare httpclient private field
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        var products = await productRepository.GetProductsAsync();
        // get products with applied calculation of price, and mapped to currency
        return products.MapToResponse();
    }
}