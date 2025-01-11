using MediatR;

namespace ProductService.Application.Products.DeleteProducts;
public record DeleteProductCommand(Guid Id) : IRequest;