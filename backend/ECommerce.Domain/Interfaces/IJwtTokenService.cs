using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

// See the note in IPasswordHasher.cs on why this is declared in Domain rather
// than Infrastructure. Concrete implementation lives in
// ECommerce.Infrastructure/Authentication.
public interface IJwtTokenService
{
    // customerId: every User in this schema has a linked Customer row
    // (created together at registration, see the seed data for admin too),
    // so cart/order/review endpoints can read it straight from the token
    // instead of doing a User->Customer lookup on every request.
    (string Token, int ExpiresInSeconds) GenerateToken(User user, Guid customerId);
}
