using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the CreateOrderDto class.
/// </summary>

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required.")
            .SetValidator(new AddressDtoValidator());

        RuleFor(x => x.BillingAddress)
            .NotNull().WithMessage("Billing address is required.")
            .SetValidator(new AddressDtoValidator()); 

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemDtoValidator());
    }
}