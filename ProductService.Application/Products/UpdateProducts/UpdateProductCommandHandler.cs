using MediatR;

namespace ProductService.Application.Products.UpdateProducts;
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Guid>
{
    // Declare repository private field
    // Call main ctor and initialize repository
    public Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // modify product
        // Add product to db
        // Persist in db
        // trigger event
        // return result if needed
        throw new NotImplementedException();
    }
}
