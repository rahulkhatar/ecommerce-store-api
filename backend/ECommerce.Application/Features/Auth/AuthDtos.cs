namespace ECommerce.Application.Features.Auth;

public record RegisterDto(string Email, string Password, string FirstName, string LastName, string? PhoneNumber);

public record LoginDto(string Email, string Password);

public record UserDto(Guid Id, string Email, string FirstName, string LastName, string Role);

public record AuthResponseDto(string Token, int ExpiresIn, UserDto User);
