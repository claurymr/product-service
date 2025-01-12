using FluentValidation.Results;

namespace ProductService.Application.Validation;
public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
{
    public ValidationFailed(ValidationFailure error) : this(new[] { error })
    {
    }
}