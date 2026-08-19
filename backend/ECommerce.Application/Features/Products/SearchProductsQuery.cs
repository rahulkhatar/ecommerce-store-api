using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Products;

public record ProductSearchHitDto(Guid ProductId, string Name, string CategoryName, decimal Price, string? ImageUrl, double Score);

public record SearchProductsQuery(string Query, int Top) : IRequest<List<ProductSearchHitDto>>;

// This is hybrid (keyword + vector) search via Elasticsearch, not the plain
// SQL filtering GetProductsQuery does - see IProductSearchService and
// ElasticsearchProductSearchService. Requires an OpenAI API key configured
// (for query embedding) and a product to have been indexed via
// CreateProductCommand; without either, this throws rather than silently
// returning SQL results, so callers can tell the difference.
public class SearchProductsQueryHandler(IProductSearchService searchService) : IRequestHandler<SearchProductsQuery, List<ProductSearchHitDto>>
{
    public async Task<List<ProductSearchHitDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var top = request.Top is < 1 or > 50 ? 10 : request.Top;
        var results = await searchService.SearchAsync(request.Query, top, cancellationToken);
        return results.Select(r => new ProductSearchHitDto(r.ProductId, r.Name, r.CategoryName, r.Price, r.ImageUrl, r.Score)).ToList();
    }
}
