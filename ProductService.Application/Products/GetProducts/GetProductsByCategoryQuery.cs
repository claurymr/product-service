using MediatR;

namespace ProductService.Application.Products.GetProducts;
public record GetProductsByCategoryQuery(
    string Category, 
    string? Currency = null) 
    : IRequest<IEnumerable<ProductQueryResponse>>;