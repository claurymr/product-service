using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.CreateProducts;
/// <summary>
/// Command to create a new product.
/// </summary>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">The description of the product.</param>
/// <param name="Price">The price of the product.</param>
/// <param name="Category">The category of the product.</param>
/// <param name="Sku">The stock keeping unit (SKU) of the product.</param>
/// <returns>A result containing the unique identifier of the created product or a validation failure.</returns>
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Category,
    string Sku)
    : IRequest<Result<Guid, ValidationFailed>>;
