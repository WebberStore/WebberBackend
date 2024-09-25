﻿using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
///Validator for the ProductVariantOptionDto class.
/// </summary>

public class ProductVariantOptionDtoValidator : AbstractValidator<ProductVariantOptionDto>
{
    public ProductVariantOptionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Attribute name is required.")
            .MaximumLength(50).WithMessage("Attribute name cannot exceed 50 characters.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Attribute value is required.")
            .MaximumLength(100).WithMessage("Attribute value cannot exceed 100 characters.");
    }
}