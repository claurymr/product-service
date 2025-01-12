using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.Products.GetProducts;
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    // Declare repository private field
    // Declare httpclient private field
    // Call main ctor and initialize repository and httpclient
    public Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        // get products by id with applied calculation of price, and mapped to currency
        // return list of products
        throw new NotImplementedException();
    }
}