using System.Text.Json;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Caching;

// Caching is an optimization, not a source of truth - a Redis outage should
// degrade to "always hit the DB", not break the API. Every method swallows
// and logs rather than letting a connection failure surface to the caller.
public class RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await cache.GetStringAsync(key, cancellationToken);
            return json is null ? default : JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache read failed for key {Key}; falling through to the source of truth.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache write failed for key {Key}.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache removal failed for key {Key}.", key);
        }
    }
}
