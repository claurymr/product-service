using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.DeleteProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Handlers.Products.DeleteProducts;
public class DeleteProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, Result<Guid, RecordNotFound>>
{
    // Declare repository private field
    // Call main ctor and initialize repository
    public async Task<Result<Guid, RecordNotFound>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // delete product from db
        var deleted = await productRepository.DeleteProductAsync(request.Id);
        if (deleted == Guid.Empty)
        {
            return new RecordNotFound([$"Product with Id {request.Id} not found."]);
        }
        // trigger event
        return deleted;
    }
}