using ECommerce.Domain.Interfaces;
using MediatR;

namespace ECommerce.Application.Features.Products;

public record ProductSearchHitDto(Guid ProductId, string Name, string CategoryName, decimal Price, string? ImageUrl, double Score, string? Description);

public record SearchProductsQuery(string Query, int Top) : IRequest<List<ProductSearchHitDto>>;

// Plain SQL LIKE match on name/description/category/vendor - see
// ProductRepository.SearchAsync. This used to be one of three strategies
// (Filter/Semantic/Hybrid) behind Elasticsearch; the semantic/hybrid modes
// were removed along with the Elasticsearch dependency, leaving this as the
// only search path.
public class SearchProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<SearchProductsQuery, List<ProductSearchHitDto>>
{
    public async Task<List<ProductSearchHitDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var top = request.Top is < 1 or > 50 ? 10 : request.Top;

        var results = await productRepository.SearchAsync(request.Query, top, cancellationToken);
        return results.Select(p => new ProductSearchHitDto(p.Id, p.Name, p.Category.Name, p.Price, p.ImageUrl, Score: 0, p.Description)).ToList();
    }
}
