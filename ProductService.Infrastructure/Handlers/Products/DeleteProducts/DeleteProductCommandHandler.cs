using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.EventBus;
using ProductService.Application.Products.DeleteProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Handlers.Products.DeleteProducts;
public class DeleteProductCommandHandler(IProductRepository productRepository, IEventBus eventBus)
    : IRequestHandler<DeleteProductCommand, Result<Guid, RecordNotFound>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IEventBus _eventBus = eventBus;

    public async Task<Result<Guid, RecordNotFound>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _productRepository.DeleteProductAsync(request.Id);
        if (deleted == Guid.Empty)
        {
            return new RecordNotFound([$"Product with Id {request.Id} not found."]);
        }

        // Publish the product created event to the message broker.
        await _eventBus.PublishAsync(
                new ProductDeletedEvent
                {
                    Id = deleted
                }, cancellationToken);
        return deleted;
    }
}