using Microsoft.Extensions.VectorData;

namespace ECommerce.Infrastructure.Search;

// The Elasticsearch-side schema for hybrid product search - deliberately
// separate from the Domain Product entity (SQL Server via EF Core stays the
// source of truth; this is a derived, denormalized search index rebuilt from
// it). SearchText is what gets embedded into the vector field: with
// EmbeddingGenerator configured on the collection (see DependencyInjection),
// assigning it a plain string is enough - the connector generates the
// embedding automatically on upsert, and HybridSearchAsync accepts a plain
// query string the same way for the search side.
public class ProductSearchDocument
{
    [VectorStoreKey]
    public Guid Id { get; set; }

    [VectorStoreData(IsFullTextIndexed = true)]
    public string Name { get; set; } = null!;

    [VectorStoreData]
    public string CategoryName { get; set; } = null!;

    [VectorStoreData]
    public decimal Price { get; set; }

    [VectorStoreData]
    public string? ImageUrl { get; set; }

    [VectorStoreVector(1536)]
    public string SearchText { get; set; } = null!;
}
