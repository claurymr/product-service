using MediatR;

namespace ProductService.Application.Products.GetProducts;
public record GetAllProductsQuery(string? Currency = null) : IRequest<IEnumerable<ProductQueryResponse>>;
