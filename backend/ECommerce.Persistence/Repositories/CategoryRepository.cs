using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class CategoryRepository(ECommerceDbContext context) : ICategoryRepository
{
    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        => context.Categories.Where(c => c.IsDeleted != true).OrderBy(c => c.DisplayOrder).ToListAsync(cancellationToken);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted != true, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
        => context.Categories.AnyAsync(c => c.Slug == slug, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        => await context.Categories.AddAsync(category, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
