using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Products;

public record GetVendorsQuery(Guid? CategoryId) : IRequest<List<string>>;

public class GetVendorsQueryHandler(IProductRepository productRepository, ICacheService cache)
    : IRequestHandler<GetVendorsQuery, List<string>>
{
    public async Task<List<string>> Handle(GetVendorsQuery request, CancellationToken cancellationToken)
    {
        var version = await ProductCacheKeys.GetVersionAsync(cache, cancellationToken);
        var cacheKey = ProductCacheKeys.VendorsKey(version, request.CategoryId);

        var cached = await cache.GetAsync<List<string>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var vendors = await productRepository.GetDistinctVendorsAsync(request.CategoryId, cancellationToken);
        await cache.SetAsync(cacheKey, vendors, ProductCacheKeys.Ttl, cancellationToken);
        return vendors;
    }
}
