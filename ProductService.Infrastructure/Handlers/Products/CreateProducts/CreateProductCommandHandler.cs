using FluentValidation;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Mappings;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;

namespace ProductService.Infrastructure.Handlers.Products.CreateProducts;

public class CreateProductCommandHandler(IProductRepository productRepository, IValidator<CreateProductCommand> _validator)
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
        // trigger event
        return productId;
    }
}
