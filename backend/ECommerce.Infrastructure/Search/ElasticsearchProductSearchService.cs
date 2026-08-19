using ECommerce.Domain.Interfaces;
using Elastic.SemanticKernel.Connectors.Elasticsearch;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Search;

public class ElasticsearchProductSearchService(
    ElasticsearchCollection<Guid, ProductSearchDocument> collection,
    ILogger<ElasticsearchProductSearchService> logger) : IProductSearchService
{
    // Registered as a singleton (see DependencyInjection.cs), so this only
    // actually runs once per process, not once per request.
    private bool _ensured;

    public async Task IndexAsync(
        Guid productId, string name, string description, string categoryName, decimal price, string? imageUrl,
        CancellationToken cancellationToken = default)
    {
        await EnsureCollectionAsync(cancellationToken);

        var document = new ProductSearchDocument
        {
            Id = productId,
            Name = name,
            CategoryName = categoryName,
            Price = price,
            ImageUrl = imageUrl,
            // What actually gets embedded - name weighted first since it's
            // usually the strongest relevance signal for a short catalog query.
            SearchText = $"{name}. {description}",
        };

        await collection.UpsertAsync(document, cancellationToken);
        logger.LogInformation("Indexed product {ProductId} into Elasticsearch.", productId);
    }

    public async Task<List<ProductSearchResult>> SearchAsync(string query, int top, CancellationToken cancellationToken = default)
    {
        await EnsureCollectionAsync(cancellationToken);

        var results = new List<ProductSearchResult>();
        await foreach (var hit in collection.HybridSearchAsync(query, [query], top, cancellationToken: cancellationToken))
        {
            results.Add(new ProductSearchResult(
                hit.Record.Id, hit.Record.Name, hit.Record.CategoryName, hit.Record.Price, hit.Record.ImageUrl, hit.Score ?? 0));
        }

        return results;
    }

    private async Task EnsureCollectionAsync(CancellationToken cancellationToken)
    {
        if (_ensured)
        {
            return;
        }

        await collection.EnsureCollectionExistsAsync(cancellationToken);
        _ensured = true;
    }
}
