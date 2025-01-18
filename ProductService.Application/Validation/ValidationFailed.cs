using FluentValidation.Results;

namespace ProductService.Application.Validation;
/// <summary>
/// Represents a validation failure that contains a collection of validation errors.
/// </summary>
/// <param name="Errors">A collection of <see cref="ValidationFailure"/> objects representing the validation errors.</param>
public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
{
    public ValidationFailed(ValidationFailure error) : this([error])
    {
    }
}