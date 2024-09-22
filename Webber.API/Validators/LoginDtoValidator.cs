﻿using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;

/// <summary>
/// Validator for the LoginDto class.
/// </summary>

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}