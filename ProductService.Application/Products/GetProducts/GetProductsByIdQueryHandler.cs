using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.Products.GetProducts;
public class GetProductsByIdQueryHandler : IRequestHandler<GetProductsByIdQuery, IEnumerable<ProductResponse>>
{
    // Declare repository private field
    // Declare httpclient private field
    // Call main ctor and initialize repository and httpclient
    public Task<IEnumerable<ProductResponse>> Handle(GetProductsByIdQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        // get products by id with applied calculation of price, and mapped to currency
        // return list of products
        throw new NotImplementedException();
    }
}