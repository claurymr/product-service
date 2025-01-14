using FluentValidation;
using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Application.Repositories;
using ProductService.Application.Validation;
using ProductService.Application.Mappings;

namespace ProductService.Infrastructure.Handlers.Products.UpdateProducts;
public class UpdateProductCommandHandler(IProductRepository productRepository, IValidator<UpdateProductCommand> _validator)
    : IRequestHandler<UpdateProductCommand, ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
{
    public async Task<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var product = request.MapToDomain();
        var updated = await productRepository.UpdateProductAsync(request.Id, product);

        if (updated == Guid.Empty)
        {
            return new RecordNotFound([$"Product with Id {request.Id} not found."]);
        }

        // trigger event

        return updated;
    }
}
