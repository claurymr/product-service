using FluentValidation.Results;
using ProductService.Application.Contracts;
using ProductService.Application.Products.CreateProducts;
using ProductService.Application.Products.UpdateProducts;
using ProductService.Application.Validation;
using ProductService.Domain;

namespace ProductService.Application.Mappings;
public static class ContractMapping
{

    public static ValidationFailureResponse MapToResponse(this IEnumerable<ValidationFailure> validationFailures)
    {
        return new ValidationFailureResponse
        {
            Errors = validationFailures.Select(x => new ValidationResponse
            {
                PropertyName = x.PropertyName,
                Message = x.ErrorMessage
            })
        };
    }

    public static ValidationFailureResponse MapToResponse(this ValidationFailed failed)
    {
        return new ValidationFailureResponse
        {
            Errors = failed.Errors.Select(x => new ValidationResponse
            {
                PropertyName = x.PropertyName,
                Message = x.ErrorMessage
            })
        };
    }

    public static OperationFailureResponse MapToResponse(this RecordNotFound notFound)
    {
        return new OperationFailureResponse
        {
            Errors = notFound.Messages.Select(message => new OperationResponse
            {
                Message = message
            })
        };
    }

    public static ProductResponse MapToResponse(this Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            Sku = product.Sku
        };
    }

    public static IEnumerable<ProductResponse> MapToResponse(this IEnumerable<Product> products)
    {
        return products.Select(MapToResponse);
    }

    public static PriceHistoryResponse MapToResponse(this PriceHistory priceHistory)
    {
        return new PriceHistoryResponse
        {
            Id = priceHistory.Id,
            ProductId = priceHistory.ProductId,
            ProductName = priceHistory.Product.Name,
            ProductSku = priceHistory.Product.Sku,
            OldPrice = priceHistory.OldPrice,
            NewPrice = priceHistory.NewPrice,
            Action = (Enums.ActionType)priceHistory.Action,
            Timestamp = priceHistory.Timestamp
        };
    }

    public static IEnumerable<PriceHistoryResponse> MapToResponse(this IEnumerable<PriceHistory> priceHistories)
    {
        return priceHistories.Select(MapToResponse);
    }

    public static Product MapToDomain(this CreateProductCommand request)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Sku = request.Sku
        };
    }

    public static Product MapToDomain(this UpdateProductCommand request)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Sku = request.Sku
        };
    }
}