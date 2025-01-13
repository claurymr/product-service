using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Repositories;

namespace ProductService.Application.Products.GetProducts;
public class GetProductsByCategoryQueryHandler(IProductRepository productRepository) 
    : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductResponse>>
{
    // Declare repository private field
    // Declare httpclient private field
    // Call main ctor and initialize repository and httpclient
    public Task<IEnumerable<ProductResponse>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        // get products by category with applied calculation of price, and mapped to currency
        // return list of products
        throw new NotImplementedException();
    }
}