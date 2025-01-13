using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.DeleteProducts;
public record DeleteProductCommand(Guid Id)
    : IRequest<Result<Guid, RecordNotFound>>;