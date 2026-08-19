using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Auth;

public record LoginCommand(LoginDto Dto) : IRequest<AuthResponseDto>;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var user = await userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user is null || !passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            // Same message for both cases - don't reveal whether the email exists.
            throw new NotFoundException("Invalid email or password.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.SaveChangesAsync(cancellationToken);

        // Every registration path creates a linked Customer row (including
        // the seeded admin) - a null here means a data-integrity bug, not a
        // client error, so let it surface as a 500 rather than a DomainException.
        var customerId = user.Customer?.Id
            ?? throw new InvalidOperationException($"User '{user.Id}' has no linked Customer record.");

        var (token, expiresIn) = jwtTokenService.GenerateToken(user, customerId);
        return new AuthResponseDto(token, expiresIn, new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role));
    }
}
