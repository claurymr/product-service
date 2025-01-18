using FluentValidation;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Application.Mappings;
using ProductService.Domain.Enums;
using ProductService.Application.EventBus;
using Shared.Contracts.Events;

namespace ProductService.Infrastructure.Handlers.Products.UpdateProducts;
public class UpdateProductCommandHandler
    (IProductRepository productRepository,
    IPriceHistoryRepository priceHistoryRepository,
    IValidator<UpdateProductCommand> validator,
    IEventBus eventBus)
    : IRequestHandler<UpdateProductCommand, ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPriceHistoryRepository _priceHistoryRepository = priceHistoryRepository;
    private readonly IValidator<UpdateProductCommand> _validator = validator;
    private readonly IEventBus _eventBus = eventBus;

    public async Task<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var product = request.MapToDomain();
        var (ProductId, OldPrice) = await _productRepository.UpdateProductAsync(request.Id, product);

        if (ProductId == Guid.Empty)
        {
            return new RecordNotFound([$"Product with Id {request.Id} not found."]);
        }

        // Add new price history for the updated product with the new price and action type as update.
        var newPrice = product.Price;
        var actionType = newPrice > 0 
                        ? (newPrice > OldPrice ? ActionType.Increased : ActionType.Reduced) 
                        : ActionType.Exit;
        await AddProductPriceHistory(ProductId, OldPrice, newPrice, actionType);

        // Publish the product created event to the message broker.
        await _eventBus.PublishAsync(
                new ProductUpdatedEvent
                {
                    Id = ProductId,
                    ProductName = product.Name
                }, cancellationToken);
        return ProductId;
    }

    private async Task AddProductPriceHistory(Guid productId, decimal oldPrice, decimal newPrice, ActionType actionType)
    {
        await _priceHistoryRepository.CreatePriceHistoryByProductIdAsync(productId, oldPrice, newPrice, actionType);
    }
}
