using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public record ProductPage(List<Product> Items, int TotalCount);

public interface IProductRepository
{
    Task<ProductPage> GetPagedAsync(
        int page, int pageSize, Guid? categoryId, decimal? minPrice = null, decimal? maxPrice = null,
        string? vendor = null, CancellationToken cancellationToken = default);

    // Distinct, non-null vendor names in the (optionally category-scoped)
    // catalog - powers the sidebar's Brand filter, which only has anything
    // to show once products actually carry vendor data.
    Task<List<string>> GetDistinctVendorsAsync(Guid? categoryId, CancellationToken cancellationToken = default);

    // Plain SQL keyword search on name/description/category/vendor - see
    // SearchProductsQueryHandler.
    Task<List<Product>> SearchAsync(string query, int top, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
