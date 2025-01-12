using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.CreateProducts;

public class CreateProductCommandHandler
: IRequestHandler<CreateProductCommand, Result<Guid, ValidationFailed>>
{
    // Declare repository private field
    // Call main ctor and initialize repository
    public Task<Result<Guid, ValidationFailed>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Declare and initialize product object to be created
        // Add product to db
        // Persist in db
        // trigger event
        // return result if needed
        throw new NotImplementedException();
    }
}
