using FluentValidation;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.EventBus;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Domain.Enums;
using Shared.Contracts.Events;

namespace ProductService.Infrastructure.Handlers.Products.CreateProducts;

/// <summary>
/// Handles the creation of a new product.
/// </summary>
/// <param name="productRepository">The repository for managing products.</param>
/// <param name="priceHistoryRepository">The repository for managing price history.</param>
/// <param name="validator">The validator for the CreateProductCommand.</param>
/// <param name="eventBus">The event bus for publishing events.</param>
/// <returns>
/// A handler for the CreateProductCommand that returns a Result containing either the product ID or validation errors.
/// </returns>
public class CreateProductCommandHandler
    (IProductRepository productRepository,
    IPriceHistoryRepository priceHistoryRepository,
    IValidator<CreateProductCommand> validator,
    IEventBus eventBus)
        : IRequestHandler<CreateProductCommand, Result<Guid, ValidationFailed>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPriceHistoryRepository _priceHistoryRepository = priceHistoryRepository;
    private readonly IValidator<CreateProductCommand> _validator = validator;
    private readonly IEventBus _eventBus = eventBus;
    public async Task<Result<Guid, ValidationFailed>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var product = request.MapToDomain();
        var productId = await _productRepository.CreateProductAsync(product);

        // Add new price history for the created product with the initial price and action type as entry.
        await AddProductPriceHistory(productId, 0m, product.Price, ActionType.Entry);

        // Publish the product created event to the message broker.
        await _eventBus.PublishAsync(
                new ProductCreatedEvent
                {
                    Id = productId,
                    ProductName = product.Name
                }, cancellationToken);
        return productId;
    }

    private async Task AddProductPriceHistory(Guid productId, decimal oldPrice, decimal newPrice, ActionType actionType)
    {
        await _priceHistoryRepository.CreatePriceHistoryByProductIdAsync(productId, oldPrice, newPrice, actionType);
    }
}
