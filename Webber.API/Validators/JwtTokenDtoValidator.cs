using FluentValidation;
using Webber.Application.DTOs;

namespace Webber.API.Validators;
/// <summary>
/// Validator for the JwtTokenDto class.
/// </summary>
public class JwtTokenDtoValidator : AbstractValidator<JwtTokenDto>
{
    public JwtTokenDtoValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}