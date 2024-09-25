using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the CreateCategoryDto class.
/// </summary>

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Category description cannot exceed 500 characters.");
    }
}