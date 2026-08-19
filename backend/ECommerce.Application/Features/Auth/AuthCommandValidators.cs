using FluentValidation;

namespace ECommerce.Application.Features.Auth;

// The MediatR ValidationBehavior validates the command type it receives
// (RegisterCommand/LoginCommand), not the inner Dto directly - these wrap the
// Dto validators so the pipeline picks them up automatically.
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Dto).SetValidator(new RegisterDtoValidator());
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Dto).SetValidator(new LoginDtoValidator());
    }
}
