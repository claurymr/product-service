using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.UpdateProducts;

/// <summary>
/// Command to update an existing product.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">The description of the product.</param>
/// <param name="Price">The price of the product.</param>
/// <param name="Category">The category of the product.</param>
/// <param name="Sku">The stock keeping unit (SKU) of the product.</param>
/// <returns>A result containing the updated product's ID, or validation errors, or a record not found error.</returns>
public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    string Sku)
    : IRequest<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>;
