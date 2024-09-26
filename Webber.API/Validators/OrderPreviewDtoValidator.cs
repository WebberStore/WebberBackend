﻿using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the OrderPreviewDto class.
/// </summary>

public class OrderPreviewDtoValidator : AbstractValidator<OrderPreviewDto>
{
    public OrderPreviewDtoValidator()
    {
        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required.")
            .SetValidator(new AddressDtoValidator());

        RuleFor(x => x.BillingAddress)
            .NotNull().WithMessage("Billing address is required.")
            .SetValidator(new AddressDtoValidator()); 

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemDtoValidator());
    }
}