using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public record ProductPage(List<Product> Items, int TotalCount);

public interface IProductRepository
{
    Task<ProductPage> GetPagedAsync(int page, int pageSize, Guid? categoryId, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
