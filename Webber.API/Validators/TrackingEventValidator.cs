using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
///     Validator for the TrackingEvent class.
/// </summary>

public class TrackingEventValidator : AbstractValidator<TrackingEvent>
{
    public TrackingEventValidator()
    {
        RuleFor(x => x.Timestamp)
            .NotEmpty().WithMessage("Timestamp is required.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}