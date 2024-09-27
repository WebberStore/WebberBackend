using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the CreateCheckoutSessionDto class.
/// </summary>

public class CreateCheckoutSessionDtoValidator : AbstractValidator<CreateCheckoutSessionDto>
{
    public CreateCheckoutSessionDtoValidator()
    {
        RuleFor(x => x.SuccessUrl)
            .NotEmpty().WithMessage("Success URL is required.");

        RuleFor(x => x.CancelUrl)
            .NotEmpty().WithMessage("Cancel URL is required.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required.");

        RuleForEach(x => x.CartItems)
            .SetValidator(new CartCheckoutItemDtoValidator());
    }
}