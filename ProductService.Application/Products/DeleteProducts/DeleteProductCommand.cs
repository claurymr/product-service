using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

namespace ProductService.Application.Products.DeleteProducts;
/// <summary>
/// Command to delete a product by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the product to be deleted.</param>
/// <returns>A result containing the unique identifier of the deleted product or a record not found error.</returns>
public record DeleteProductCommand(Guid Id)
    : IRequest<Result<Guid, RecordNotFound>>;
