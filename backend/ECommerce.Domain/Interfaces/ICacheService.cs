namespace ECommerce.Domain.Interfaces;

// Declared in Domain for the same reason as IPasswordHasher/IJwtTokenService -
// Infrastructure references Application, so Application handlers can't depend
// on an Infrastructure-declared interface.
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
