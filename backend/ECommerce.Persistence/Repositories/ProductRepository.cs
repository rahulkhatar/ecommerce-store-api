using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class ProductRepository(ECommerceDbContext context) : IProductRepository
{
    public async Task<ProductPage> GetPagedAsync(int page, int pageSize, Guid? categoryId, CancellationToken cancellationToken = default)
    {
        var query = context.Products.Include(p => p.Category).Where(p => p.IsDeleted != true && p.IsActive == true);

        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new ProductPage(items, totalCount);
    }

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
