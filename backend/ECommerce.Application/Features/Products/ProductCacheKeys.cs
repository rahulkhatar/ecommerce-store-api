using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Features.Products;

// Cache invalidation via a version counter rather than pattern-deleting keys:
// IDistributedCache has no "delete by prefix" operation, and scanning Redis
// keys by pattern in production is a footgun (blocks the server on a large
// keyspace). Bumping the version instead makes every previously-cached
// listing/detail key "disappear" (a cache miss) in O(1), and old entries just
// expire naturally via their TTL.
internal static class ProductCacheKeys
{
    private const string VersionKey = "products:cache-version";
    public static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public static async Task<int> GetVersionAsync(ICacheService cache, CancellationToken cancellationToken)
        => await cache.GetAsync<int?>(VersionKey, cancellationToken) ?? 0;

    public static async Task BumpVersionAsync(ICacheService cache, CancellationToken cancellationToken)
    {
        var next = await GetVersionAsync(cache, cancellationToken) + 1;
        await cache.SetAsync(VersionKey, next, TimeSpan.FromDays(30), cancellationToken);
    }

    public static string ListKey(int version, int page, int pageSize, Guid? categoryId, decimal? minPrice = null, decimal? maxPrice = null, string? vendor = null)
        => $"products:v{version}:list:{page}:{pageSize}:{categoryId?.ToString() ?? "all"}:{minPrice?.ToString() ?? "-"}:{maxPrice?.ToString() ?? "-"}:{vendor ?? "-"}";

    public static string VendorsKey(int version, Guid? categoryId)
        => $"products:v{version}:vendors:{categoryId?.ToString() ?? "all"}";

    public static string DetailKey(int version, Guid id)
        => $"products:v{version}:detail:{id}";
}
