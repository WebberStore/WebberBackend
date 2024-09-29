using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// 
/// Validator for the ShippingRate class.
/// </summary>

public class ShippingRateValidator : AbstractValidator<ShippingRate>
{
    public ShippingRateValidator()
    {
        RuleFor(x => x.Carrier)
            .NotEmpty().WithMessage("Carrier is required.");

        RuleFor(x => x.ServiceName)
            .NotEmpty().WithMessage("Service name is required.");

        RuleFor(x => x.TotalRate)
            .GreaterThanOrEqualTo(0).WithMessage("Total rate must be non-negative.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.");

        RuleFor(x => x.EstimatedDeliveryDate)
            .NotEmpty().WithMessage("Estimated delivery date is required.");
    }
}