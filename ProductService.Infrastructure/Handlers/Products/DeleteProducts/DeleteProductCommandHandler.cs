using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.DeleteProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Handlers.Products.DeleteProducts;
public class DeleteProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, Result<Guid, RecordNotFound>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<Result<Guid, RecordNotFound>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _productRepository.DeleteProductAsync(request.Id);
        if (deleted == Guid.Empty)
        {
            return new RecordNotFound([$"Product with Id {request.Id} not found."]);
        }
        return deleted;
    }
}