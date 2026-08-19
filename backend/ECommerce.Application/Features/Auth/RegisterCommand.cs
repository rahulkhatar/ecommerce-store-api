using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Auth;

public record RegisterCommand(RegisterDto Dto) : IRequest<AuthResponseDto>;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (await userRepository.EmailExistsAsync(dto.Email, cancellationToken))
        {
            throw new BusinessException($"An account with email '{dto.Email}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = passwordHasher.Hash(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Role = "Customer",
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            PhoneNumber = dto.PhoneNumber,
            TotalSpending = 0,
            LoyaltyPoints = 0,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
        };

        await userRepository.AddWithCustomerAsync(user, customer, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var (token, expiresIn) = jwtTokenService.GenerateToken(user);
        return new AuthResponseDto(token, expiresIn, new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role));
    }
}
