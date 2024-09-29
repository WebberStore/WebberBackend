using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the Shipment class.
/// </summary>

public class ShipmentValidator : AbstractValidator<Shipment>
{
    public ShipmentValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("Tracking number is required.");

        RuleFor(x => x.ShippingLabelUrls)
            .NotEmpty().WithMessage("Shipping label URLs are required.");
    }
}