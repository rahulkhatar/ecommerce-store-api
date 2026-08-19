using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception)
        {
            // A malformed/legacy hash should fail auth, not 500 the request.
            return false;
        }
    }
}
