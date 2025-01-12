using MediatR;
using ProductService.Application.Contracts;

namespace ProductService.Application.Products.GetProducts;
public record GetAllProductsQuery(string? Currency = null) : IRequest<IEnumerable<ProductResponse>>;
