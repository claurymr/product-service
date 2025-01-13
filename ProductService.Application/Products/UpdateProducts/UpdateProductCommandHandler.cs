using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.UpdateProducts;
public class UpdateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductCommand, ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
{

    // Declare repository private field
    // Call main ctor and initialize repository
    public Task<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // modify product
        // Add product to db
        // Persist in db
        // trigger event
        // return result if needed
        throw new NotImplementedException();
    }
}
