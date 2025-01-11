using MediatR;

namespace ProductService.Application.Products.CreateProducts;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    // Declare repository private field
    // Call main ctor and initialize repository
    public Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Declare and initialize product object to be created
        // Add product to db
        // Persist in db
        // trigger event
        // return result if needed
        throw new NotImplementedException();
    }
}
