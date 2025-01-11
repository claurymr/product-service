using MediatR;
namespace ProductService.Application.Products.GetProducts;
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductQueryResponse>>
{
    // Declare repository private field
    // Declare httpclient private field
    // Call main ctor and initialize repository and httpclient
    public Task<IEnumerable<ProductQueryResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // get currency if present, from external api using httpclient
        // get products with applied calculation of price, and mapped to currency
        // return list of products
        throw new NotImplementedException();
    }
}