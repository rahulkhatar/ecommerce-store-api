using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class ShoppingCartRepository(ECommerceDbContext context) : IShoppingCartRepository
{
    public Task<List<ShoppingCart>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        => context.ShoppingCarts.Include(c => c.Product)
            .Where(c => c.CustomerId == customerId && c.IsDeleted != true)
            .ToListAsync(cancellationToken);

    public Task<ShoppingCart?> GetItemAsync(Guid customerId, Guid productId, CancellationToken cancellationToken = default)
        => context.ShoppingCarts.FirstOrDefaultAsync(
            c => c.CustomerId == customerId && c.ProductId == productId && c.IsDeleted != true, cancellationToken);

    public Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
        => context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsDeleted != true, cancellationToken);

    public async Task AddAsync(ShoppingCart item, CancellationToken cancellationToken = default)
        => await context.ShoppingCarts.AddAsync(item, cancellationToken);

    public void Remove(ShoppingCart item) => context.ShoppingCarts.Remove(item);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
