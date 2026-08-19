using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IShoppingCartRepository
{
    Task<List<ShoppingCart>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<ShoppingCart?> GetItemAsync(Guid customerId, Guid productId, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(ShoppingCart item, CancellationToken cancellationToken = default);
    void Remove(ShoppingCart item);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
