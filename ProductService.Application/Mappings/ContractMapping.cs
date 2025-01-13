using FluentValidation.Results;
using ProductService.Application.Contracts;
using ProductService.Application.Validation;

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
}