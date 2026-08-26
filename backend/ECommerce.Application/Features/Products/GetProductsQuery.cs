using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Products;

public record GetProductsQuery(int Page, int PageSize, Guid? CategoryId, decimal? MinPrice = null, decimal? MaxPrice = null, string? Vendor = null)
    : IRequest<PagedResult<ProductDto>>;

public class GetProductsQueryHandler(IProductRepository productRepository, ICacheService cache)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var version = await ProductCacheKeys.GetVersionAsync(cache, cancellationToken);
        var cacheKey = ProductCacheKeys.ListKey(version, page, pageSize, request.CategoryId, request.MinPrice, request.MaxPrice, request.Vendor);

        var cached = await cache.GetAsync<PagedResult<ProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var result = await productRepository.GetPagedAsync(
            page, pageSize, request.CategoryId, request.MinPrice, request.MaxPrice, request.Vendor, cancellationToken);
        var dto = new PagedResult<ProductDto>(result.Items.Select(p => p.ToDto()).ToList(), page, pageSize, result.TotalCount);

        await cache.SetAsync(cacheKey, dto, ProductCacheKeys.Ttl, cancellationToken);
        return dto;
    }
}
