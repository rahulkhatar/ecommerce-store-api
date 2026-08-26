using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class ProductRepository(ECommerceDbContext context) : IProductRepository
{
    public async Task<ProductPage> GetPagedAsync(
        int page, int pageSize, Guid? categoryId, decimal? minPrice = null, decimal? maxPrice = null,
        string? vendor = null, CancellationToken cancellationToken = default)
    {
        var query = context.Products.Include(p => p.Category).Where(p => p.IsDeleted != true && p.IsActive == true);

        // Matches the category itself OR any of its direct subcategories, so
        // browsing a parent department (e.g. Sports & Outdoors) still
        // aggregates products that live under its type subcategories
        // (Skates, Football, ...) instead of coming back empty.
        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId || p.Category.ParentCategoryId == categoryId);
        }

        // Filtered on what the customer actually pays, not the list price -
        // a discounted item under the cap should still show up.
        if (minPrice is not null)
        {
            query = query.Where(p => (p.DiscountPrice ?? p.Price) >= minPrice);
        }
        if (maxPrice is not null)
        {
            query = query.Where(p => (p.DiscountPrice ?? p.Price) <= maxPrice);
        }
        if (!string.IsNullOrWhiteSpace(vendor))
        {
            query = query.Where(p => p.Vendor == vendor);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new ProductPage(items, totalCount);
    }

    public Task<List<string>> GetDistinctVendorsAsync(Guid? categoryId, CancellationToken cancellationToken = default)
    {
        var query = context.Products.Where(p => p.IsDeleted != true && p.IsActive == true && p.Vendor != null);

        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        return query.Select(p => p.Vendor!).Distinct().OrderBy(v => v).ToListAsync(cancellationToken);
    }

    public Task<List<Product>> SearchAsync(string query, int top, CancellationToken cancellationToken = default)
        => context.Products.Include(p => p.Category)
            .Where(p => p.IsDeleted != true && p.IsActive == true
                && (EF.Functions.Like(p.Name, $"%{query}%")
                    || EF.Functions.Like(p.Description, $"%{query}%")
                    // This started life as just the Elasticsearch-outage
                    // fallback, matching Name/Description only - now that
                    // "Filter" is its own selectable search mode (not just a
                    // safety net), a literal/keyword search that can't even
                    // match a product's own category name (searching
                    // "electronics" not finding Electronics products) isn't
                    // a useful keyword search, it's just broken-feeling.
                    || EF.Functions.Like(p.Category.Name, $"%{query}%")
                    || EF.Functions.Like(p.Vendor, $"%{query}%")))
            // Name matches are usually more relevant than a description or
            // category-name mention - simple heuristic since plain SQL LIKE
            // has no real relevance scoring the way Elasticsearch does.
            .OrderByDescending(p => EF.Functions.Like(p.Name, $"%{query}%"))
            .ThenByDescending(p => p.CreatedAt)
            .Take(top)
            .ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted != true, cancellationToken);

    public Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
        => context.Products.AnyAsync(p => p.Sku == sku, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
        => context.Products.AnyAsync(p => p.Slug == slug, cancellationToken);

    public Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => context.Categories.AnyAsync(c => c.Id == categoryId && c.IsDeleted != true, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await context.Products.AddAsync(product, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
