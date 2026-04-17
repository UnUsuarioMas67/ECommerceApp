using ECommerce.Api.DTOs.Auth;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Auth;

public class RefreshRequestDtoValidator : AbstractValidator<RefreshRequestDto>
{
    public RefreshRequestDtoValidator()
    {
        RuleFor(p => p.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}