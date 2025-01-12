using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.UpdateProducts;
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<bool, ValidationFailed>>
{

    // Declare repository private field
    // Call main ctor and initialize repository
    public Task<Result<bool, ValidationFailed>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // modify product
        // Add product to db
        // Persist in db
        // trigger event
        // return result if needed
        throw new NotImplementedException();
    }
}
