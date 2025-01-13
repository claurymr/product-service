using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.DeleteProducts;
public class DeleteProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, Result<Guid, RecordNotFound>>
{
    // Declare repository private field
    // Call main ctor and initialize repository
    public Task<Result<Guid, RecordNotFound>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // delete product from db
        // Persist in db
        // trigger event
        // return result if needed
        throw new NotImplementedException();
    }
}