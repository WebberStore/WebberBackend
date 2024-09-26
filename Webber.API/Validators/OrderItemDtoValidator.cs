﻿using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the OrderItemDto class.
/// </summary>

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1).WithMessage("Quantity must be at least 1.");
    }
}