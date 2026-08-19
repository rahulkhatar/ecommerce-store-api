using ECommerce.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Products;

public record ProductSearchHitDto(Guid ProductId, string Name, string CategoryName, decimal Price, string? ImageUrl, double Score);

public record SearchProductsQuery(string Query, int Top) : IRequest<List<ProductSearchHitDto>>;

// Hybrid (keyword + vector) search via Elasticsearch, not the plain SQL
// filtering GetProductsQuery does - see IProductSearchService and
// ElasticsearchProductSearchService. Falls back to a plain SQL LIKE search
// (IProductRepository.SearchAsync) if Elasticsearch/OpenAI is unreachable or
// not configured (e.g. no real OpenAI API key yet), so the endpoint degrades
// to "works, just not semantically" instead of a 500 - the same reasoning as
// RedisCacheService degrading to "always hit the DB".
public class SearchProductsQueryHandler(
    IProductSearchService searchService,
    IProductRepository productRepository,
    ILogger<SearchProductsQueryHandler> logger) : IRequestHandler<SearchProductsQuery, List<ProductSearchHitDto>>
{
    public async Task<List<ProductSearchHitDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var top = request.Top is < 1 or > 50 ? 10 : request.Top;

        try
        {
            var results = await searchService.SearchAsync(request.Query, top, cancellationToken);
            return results.Select(r => new ProductSearchHitDto(r.ProductId, r.Name, r.CategoryName, r.Price, r.ImageUrl, r.Score)).ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Hybrid search failed for query '{Query}'; falling back to SQL search.", request.Query);

            var fallback = await productRepository.SearchAsync(request.Query, top, cancellationToken);
            return fallback.Select(p => new ProductSearchHitDto(p.Id, p.Name, p.Category.Name, p.Price, p.ImageUrl, Score: 0)).ToList();
        }
    }
}
