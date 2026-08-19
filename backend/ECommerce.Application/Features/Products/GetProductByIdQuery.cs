using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler(IProductRepository productRepository, ICacheService cache)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var version = await ProductCacheKeys.GetVersionAsync(cache, cancellationToken);
        var cacheKey = ProductCacheKeys.DetailKey(version, request.Id);

        var cached = await cache.GetAsync<ProductDto>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product '{request.Id}' not found.");

        var dto = product.ToDto();
        await cache.SetAsync(cacheKey, dto, ProductCacheKeys.Ttl, cancellationToken);
        return dto;
    }
}
