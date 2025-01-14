using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.Products.GetProducts;
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    // Declare repository private field
    // Declare httpclient private field
    // Call main ctor and initialize repository and httpclient
    public Task<IEnumerable<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        // get products with applied calculation of price, and mapped to currency
        // return list of products
        throw new NotImplementedException();
    }
}