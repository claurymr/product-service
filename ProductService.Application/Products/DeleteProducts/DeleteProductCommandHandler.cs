using MediatR;

namespace ProductService.Application.Products.DeleteProducts;
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    // Declare repository private field
    // Call main ctor and initialize repository
    public Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // delete product from db
        // Persist in db
        // trigger event
        // return result if needed
        throw new NotImplementedException();
    }
}