﻿using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the ShippingRateRequest class.
/// </summary>

public class ShippingRateRequestValidator : AbstractValidator<ShippingRateRequest>
{
    public ShippingRateRequestValidator()
    {
        RuleFor(x => x.OriginPostalCode)
            .NotEmpty().WithMessage("Origin postal code is required.");

        RuleFor(x => x.DestinationPostalCode)
            .NotEmpty().WithMessage("Destination postal code is required.");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.");

        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Length must be greater than 0.");

        RuleFor(x => x.Width)
            .GreaterThan(0).WithMessage("Width must be greater than 0.");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0.");
    }
}