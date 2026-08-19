using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

// See the note in IPasswordHasher.cs on why this is declared in Domain rather
// than Infrastructure. Concrete implementation lives in
// ECommerce.Infrastructure/Authentication.
public interface IJwtTokenService
{
    (string Token, int ExpiresInSeconds) GenerateToken(User user);
}
