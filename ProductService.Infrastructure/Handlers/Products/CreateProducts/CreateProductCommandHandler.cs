using FluentValidation;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Domain.Enums;

namespace ProductService.Infrastructure.Handlers.Products.CreateProducts;

public class CreateProductCommandHandler
    (IProductRepository productRepository, IPriceHistoryRepository priceHistoryRepository, IValidator<CreateProductCommand> _validator)
        : IRequestHandler<CreateProductCommand, Result<Guid, ValidationFailed>>
{
    public async Task<Result<Guid, ValidationFailed>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var product = request.MapToDomain();
        var productId = await productRepository.CreateProductAsync(product);

        // Add new price history for the created product with the initial price and action type as entry.
        await AddProductPriceHistory(productId, 0m, product.Price, ActionType.Entry);

        // trigger event
        return productId;
    }

    private async Task AddProductPriceHistory(Guid productId, decimal oldPrice, decimal newPrice, ActionType actionType)
    {
        await priceHistoryRepository.CreatePriceHistoryByProductIdAsync(productId, oldPrice, newPrice, actionType);
    }
}
